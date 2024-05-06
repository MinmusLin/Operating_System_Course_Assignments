# 导入相关包和模块
import sys
import warnings
import ui_design
from enum import Enum
from datetime import datetime
from functools import partial
from PyQt5 import QtWidgets, QtGui, QtCore
from PyQt5.QtCore import QThread, QTimer, QMutex

# 忽略 DeprecationWarning 警告
warnings.filterwarnings('ignore', category=DeprecationWarning)

# 常变量
ELEVATOR_NUM = 5  # 电梯数量
FLOOR_NUM = 20  # 楼层数量
REFRESH_INTERVAL = 30  # 刷新时间间隔
THREAD_INTERVAL = 100  # 线程时间间隔
FLOOR_TRAVEL_TIME = 1000  # 电梯上下楼时间
DOOR_TOGGLE_TIME = 2000  # 电梯开关门时间
ELEVATOR_RUNNING_COLOR = (153, 204, 255)  # 电梯运行状态颜色
ELEVATOR_DOOR_COLOR = (153, 255, 153)  # 电梯开关门状态颜色
BUTTON_DEFAULT_COLOR = (255, 255, 255)  # 按钮默认状态颜色
WARNING_BUTTON_COLOR = (255, 124, 128)  # 按钮报警状态颜色
BUTTON_WARNING_STYLE = f'''
QPushButton:enabled {{
    background-color: rgb(255, 124, 128);
    color: black;
    border: 1px solid rgb(208, 208, 208);
    border-radius: 4px;
}}
'''  # 按钮报警状态（启用状态）样式
BUTTON_CLICKED_STYLE = f'''
QPushButton:disabled {{
    background-color: rgb(255, 255, 0);
    color: black;
    border: 1px solid rgb(208, 208, 208);
    border-radius: 4px;
}}
'''  # 按钮选中状态（禁用状态）样式


# 电梯运行状态枚举类
class ElevatorState(Enum):
    DOWNWARD = -1  # 电梯下行状态
    STATIONARY = 0  # 电梯静止状态
    UPWARD = 1  # 电梯上行状态
    WARNING = 2  # 电梯报警状态
    DOOR_OPERATING = 3  # 电梯开关门状态


# 电梯任务状态枚举类
class ElevatorTaskState(Enum):
    PENDING = 0  # 任务尚未分配状态
    IN_PROGRESS = 1  # 任务正在等待状态
    COMPLETED = 2  # 任务已完成状态


# 全局变量
mutex = QMutex()  # 互斥锁（管理线程同步，避免数据竞争和条件竞争）
elevator_up_queue = [[] for _ in range(ELEVATOR_NUM)]  # 每台电梯待处理的上行目标列表
elevator_down_queue = [[] for _ in range(ELEVATOR_NUM)]  # 每台电梯待处理的下行目标列表
elevator_task_list = []  # 电梯任务列表
elevator_state = [ElevatorState.STATIONARY for _ in range(ELEVATOR_NUM)]  # 每台电梯的状态
elevator_current_floor = [0 for _ in range(ELEVATOR_NUM)]  # 每台电梯的当前楼层
elevator_door_operating_progress = [0.0 for _ in range(ELEVATOR_NUM)]  # 每台电梯的开关门进度
move_directions = [ElevatorState.UPWARD for _ in range(ELEVATOR_NUM)]  # 每台电梯的运行方向


# 窗口类
class MainWindow(QtWidgets.QMainWindow):
    # noinspection PyUnresolvedReferences
    def __init__(self):
        """
        初始化窗口类
        """
        super(MainWindow, self).__init__()

        self.ui = ui_design.Ui_MainWindow()  # 初始化用户界面

        self.elevator_lcd_number = []  # 电梯楼层显示
        self.elevator_list_widget = []  # 电梯状态列表
        self.elevator_open_door_button = []  # 电梯开门按钮
        self.elevator_close_door_button = []  # 电梯关门按钮
        self.elevator_warning_button = []  # 电梯报警按钮
        self.to_floor_elevator_button = []  # 电梯楼层按钮
        self.upward_elevator_button = []  # 电梯上行按钮
        self.downward_elevator_button = []  # 电梯下行按钮

        self.console_plain_text_edit = []  # 控制台窗口

        self.init_ui_components()  # 初始化窗口

        self.timer = QTimer()  # 创建定时器实例
        self.timer.setInterval(REFRESH_INTERVAL)  # 设置定时器触发间隔
        self.timer.timeout.connect(self.update)  # 连接定时器信号
        self.timer.start()  # 启动定时器

        self.show()  # 显示窗口

    def init_ui_components(self):
        """
        初始化 UI 组件
        """
        self.ui.setupUi(self)
        self.setWindowTitle('Process Management - Elevator Scheduling | 进程管理 - 电梯调度')

        for elevator_index in range(ELEVATOR_NUM):
            self.elevator_lcd_number.append(getattr(self.ui, f'elevatorLcdNumber{1 + elevator_index:02}'))
            self.elevator_list_widget.append(getattr(self.ui, f'elevatorListWidget{1 + elevator_index:02}'))
            self.elevator_open_door_button.append(getattr(self.ui, f'elevatorOpenDoorButton{1 + elevator_index:02}'))
            self.elevator_close_door_button.append(getattr(self.ui, f'elevatorCloseDoorButton{1 + elevator_index:02}'))
            self.elevator_warning_button.append(getattr(self.ui, f'elevatorWarningButton{1 + elevator_index:02}'))

        for elevator_index in range(ELEVATOR_NUM):
            self.elevator_open_door_button[elevator_index].clicked.connect(
                partial(self.elevator_open_door_button_clicked, elevator_index))
            self.elevator_close_door_button[elevator_index].clicked.connect(
                partial(self.elevator_close_door_button_clicked, elevator_index))
            self.elevator_warning_button[elevator_index].clicked.connect(
                partial(self.elevator_warning_button_clicked, elevator_index))

        for elevator_index in range(ELEVATOR_NUM):
            elevator_button_list = []
            for floor_index in range(FLOOR_NUM):
                button = getattr(self.ui, f'toFloorElevatorButton{elevator_index + 1:02d}{floor_index + 1:02d}')
                elevator_button_list.append(button)
                button.clicked.connect(partial(self.to_floor_elevator_button_clicked, elevator_index, floor_index))
            self.to_floor_elevator_button.append(elevator_button_list)

        for floor_index in range(FLOOR_NUM):
            upward_button = getattr(self.ui, f'upwardElevatorButton{floor_index + 1:02d}')
            self.upward_elevator_button.append(upward_button)
            if floor_index != FLOOR_NUM - 1:
                upward_button.clicked.connect(
                    partial(self.upward_or_downward_elevator_button_clicked, floor_index, ElevatorState.UPWARD))

            downward_button = getattr(self.ui, f'downwardElevatorButton{floor_index + 1:02d}')
            self.downward_elevator_button.append(downward_button)
            if floor_index != 0:
                downward_button.clicked.connect(
                    partial(self.upward_or_downward_elevator_button_clicked, floor_index, ElevatorState.DOWNWARD))

        self.console_plain_text_edit = self.ui.consolePlainTextEdit

    def console_output(self, message):
        """
        控制台输出信息

        :param message: 信息
        :type message: str
        """
        now = datetime.now()
        formatted_time = now.strftime('%Y-%m-%d %H:%M:%S')
        self.console_plain_text_edit.appendPlainText(f'[{formatted_time}] {message}')

    def elevator_open_door_button_clicked(self, elevator_index):
        """
        电梯开门按钮点击事件函数

        :param elevator_index: 电梯索引
        :type elevator_index: int
        """
        if elevator_state[elevator_index] == ElevatorState.DOWNWARD:
            self.console_output(f'{elevator_index + 1}号电梯正在下行，无法开门')
        elif elevator_state[elevator_index] == ElevatorState.STATIONARY:
            self.console_output(f'{elevator_index + 1}号电梯已开门')
        elif elevator_state[elevator_index] == ElevatorState.UPWARD:
            self.console_output(f'{elevator_index + 1}号电梯正在上行，无法开门')
        elif elevator_state[elevator_index] == ElevatorState.WARNING:
            self.console_output(f'{elevator_index + 1}号电梯处于报警状态')
        elif elevator_state[elevator_index] == ElevatorState.DOOR_OPERATING:
            self.console_output(f'{elevator_index + 1}号电梯正在开关门')

    def elevator_close_door_button_clicked(self, elevator_index):
        """
        电梯关门按钮点击事件函数

        :param elevator_index: 电梯索引
        :type elevator_index: int
        """
        if elevator_state[elevator_index] == ElevatorState.DOWNWARD:
            self.console_output(f'{elevator_index + 1}号电梯正在下行，无法关门')
        elif elevator_state[elevator_index] == ElevatorState.STATIONARY:
            self.console_output(f'{elevator_index + 1}号电梯已关门')
        elif elevator_state[elevator_index] == ElevatorState.UPWARD:
            self.console_output(f'{elevator_index + 1}号电梯正在上行，无法关门')
        elif elevator_state[elevator_index] == ElevatorState.WARNING:
            self.console_output(f'{elevator_index + 1}号电梯处于报警状态')
        elif elevator_state[elevator_index] == ElevatorState.DOOR_OPERATING:
            self.console_output(f'{elevator_index + 1}号电梯正在开关门')

    def elevator_warning_button_clicked(self, elevator_index):
        """
        电梯报警按钮点击事件函数

        :param elevator_index: 电梯索引
        :type elevator_index: int
        """
        mutex.lock()

        if elevator_state[elevator_index] == ElevatorState.WARNING:
            self.console_output(f'{elevator_index + 1}号电梯退出报警状态')

            elevator_state[elevator_index] = ElevatorState.STATIONARY
            mutex.unlock()
            self.elevator_warning_button[elevator_index].setStyleSheet('')

            for button in self.to_floor_elevator_button[elevator_index]:
                button.setStyleSheet('')
                button.setEnabled(True)

            self.elevator_open_door_button[elevator_index].setEnabled(True)
            self.elevator_close_door_button[elevator_index].setEnabled(True)

            for floor_index in range(FLOOR_NUM):
                self.update_elevator_state(elevator_index, floor_index)

        else:
            self.console_output(f'{elevator_index + 1}号电梯进入报警状态')
            self.console_output(f'{elevator_index + 1}号电梯返回1层')

            elevator_state[elevator_index] = ElevatorState.WARNING
            elevator_current_floor[elevator_index] = 0
            mutex.unlock()
            self.elevator_warning_button[elevator_index].setStyleSheet(BUTTON_WARNING_STYLE)

            for button in self.to_floor_elevator_button[elevator_index]:
                button.setStyleSheet('')
                button.setEnabled(False)

            self.elevator_open_door_button[elevator_index].setEnabled(False)
            self.elevator_close_door_button[elevator_index].setEnabled(False)

            for floor_index in range(FLOOR_NUM):
                self.update_elevator_state(elevator_index, floor_index, WARNING_BUTTON_COLOR)

    def to_floor_elevator_button_clicked(self, elevator_index, floor_index):
        """
        电梯楼层按钮点击事件函数

        :param elevator_index: 电梯索引
        :type elevator_index: int
        :param floor_index: 楼层索引
        :type floor_index: int
        """
        mutex.lock()

        if elevator_state[elevator_index] == ElevatorState.WARNING:
            mutex.unlock()
            self.console_output(f'{elevator_index + 1}号电梯处于报警状态')
            return

        if floor_index == elevator_current_floor[elevator_index]:
            mutex.unlock()
            self.console_output(f'{elevator_index + 1}号电梯位于{floor_index + 1}层')
            return

        if ((floor_index > elevator_current_floor[elevator_index])
                and floor_index not in elevator_up_queue[elevator_index]):
            self.console_output(f'{elevator_index + 1}号电梯收到上行请求：前往{floor_index + 1}层')
            elevator_up_queue[elevator_index].append(floor_index)
            elevator_up_queue[elevator_index].sort()
        elif ((floor_index < elevator_current_floor[elevator_index])
              and floor_index not in elevator_down_queue[elevator_index]):
            self.console_output(f'{elevator_index + 1}号电梯收到下行请求：前往{floor_index + 1}层')
            elevator_down_queue[elevator_index].append(floor_index)
            elevator_down_queue[elevator_index].sort(reverse=True)

        self.to_floor_elevator_button[elevator_index][floor_index].setEnabled(False)
        self.to_floor_elevator_button[elevator_index][floor_index].setStyleSheet(BUTTON_CLICKED_STYLE)

        mutex.unlock()

    def upward_or_downward_elevator_button_clicked(self, floor_index, move_direction):
        """
        电梯上行或下行按钮点击事件函数

        :param floor_index: 楼层索引
        :type floor_index: int
        :param move_direction: 运行方向
        :type move_direction: ElevatorState
        """
        mutex.lock()

        all_elevator_fault = True
        for elevator_index in range(ELEVATOR_NUM):
            if elevator_state[elevator_index] != ElevatorState.WARNING:
                all_elevator_fault = False
        if all_elevator_fault:
            self.console_output('所有电梯均处于报警状态')
            mutex.unlock()
            return

        if move_direction == ElevatorState.UPWARD:
            self.console_output(f'{floor_index + 1}层发出上行请求')
        elif move_direction == ElevatorState.DOWNWARD:
            self.console_output(f'{floor_index + 1}层发出下行请求')

        task = ElevatorTask(floor_index, move_direction)
        if task not in elevator_task_list:
            elevator_task_list.append(task)

        mutex.unlock()

    def update_elevator_state(self, elevator_index, floor_index, background_color=BUTTON_DEFAULT_COLOR, message=''):
        """
        更新电梯状态

        :param elevator_index: 电梯索引
        :type elevator_index: int
        :param floor_index: 楼层索引
        :type floor_index: int
        :param background_color: 背景颜色
        :type background_color: rgb
        :param message: 信息
        :type message: str
        """
        itemBrush = QtGui.QBrush(QtGui.QColor(*background_color))
        itemBrush.setStyle(QtCore.Qt.SolidPattern)
        item = self.elevator_list_widget[elevator_index].item(FLOOR_NUM - 1 - floor_index)
        item.setBackground(itemBrush)
        item.setText(message)

    def update(self):
        """
        窗口更新函数
        """
        mutex.lock()

        for elevator_index in range(ELEVATOR_NUM):
            self.elevator_lcd_number[elevator_index].display(str(elevator_current_floor[elevator_index] + 1))
            if elevator_state[elevator_index] == ElevatorState.DOOR_OPERATING:
                self.to_floor_elevator_button[elevator_index][elevator_current_floor[elevator_index]].setEnabled(True)
                if elevator_door_operating_progress[elevator_index] < 1 / 4:
                    self.update_elevator_state(elevator_index,
                                               elevator_current_floor[elevator_index],
                                               ELEVATOR_DOOR_COLOR,
                                               '开门中')
                elif elevator_door_operating_progress[elevator_index] < 3 / 4:
                    self.update_elevator_state(elevator_index,
                                               elevator_current_floor[elevator_index],
                                               ELEVATOR_DOOR_COLOR,
                                               '等待中')
                else:
                    self.update_elevator_state(elevator_index,
                                               elevator_current_floor[elevator_index],
                                               ELEVATOR_DOOR_COLOR,
                                               '关门中')

        mutex.unlock()

        for button in self.upward_elevator_button:
            button.setEnabled(True)
            button.setStyleSheet('')
        for button in self.downward_elevator_button:
            button.setEnabled(True)
            button.setStyleSheet('')
        self.upward_elevator_button[FLOOR_NUM - 1].setEnabled(False)
        self.downward_elevator_button[0].setEnabled(False)

        mutex.lock()

        for elevator_task in elevator_task_list:
            if elevator_task.task_state != ElevatorTaskState.COMPLETED:
                if elevator_task.move_direction == ElevatorState.UPWARD:
                    self.upward_elevator_button[elevator_task.target_floor].setEnabled(False)
                    self.upward_elevator_button[elevator_task.target_floor].setStyleSheet(BUTTON_CLICKED_STYLE)
                elif elevator_task.move_direction == ElevatorState.DOWNWARD:
                    self.downward_elevator_button[elevator_task.target_floor].setEnabled(False)
                    self.downward_elevator_button[elevator_task.target_floor].setStyleSheet(BUTTON_CLICKED_STYLE)

        mutex.unlock()

        mutex.lock()

        for elevator_index in range(ELEVATOR_NUM):
            if elevator_state[elevator_index] != ElevatorState.WARNING:
                if elevator_state[elevator_index] != ElevatorState.DOOR_OPERATING:
                    if elevator_state[elevator_index] == ElevatorState.UPWARD:
                        self.update_elevator_state(elevator_index,
                                                   elevator_current_floor[elevator_index],
                                                   ELEVATOR_RUNNING_COLOR,
                                                   '上行中')
                    elif elevator_state[elevator_index] == ElevatorState.DOWNWARD:
                        self.update_elevator_state(elevator_index,
                                                   elevator_current_floor[elevator_index],
                                                   ELEVATOR_RUNNING_COLOR,
                                                   '下行中')
                    else:
                        self.update_elevator_state(elevator_index,
                                                   elevator_current_floor[elevator_index],
                                                   ELEVATOR_RUNNING_COLOR)
                if elevator_current_floor[elevator_index] > 0:
                    self.update_elevator_state(elevator_index, elevator_current_floor[elevator_index] - 1)
                if elevator_current_floor[elevator_index] < FLOOR_NUM - 1:
                    self.update_elevator_state(elevator_index, elevator_current_floor[elevator_index] + 1)

        mutex.unlock()


# 电梯任务类
class ElevatorTask:
    def __init__(self, target_floor, move_direction, task_state=ElevatorTaskState.PENDING):
        """
        初始化电梯任务类

        :param target_floor: 目标楼层
        :type target_floor: int
        :param move_direction: 电梯运行方向
        :type move_direction: ElevatorState
        :param task_state: 电梯任务状态
        :type task_state: ElevatorTaskState
        """
        self.target_floor = target_floor
        self.move_direction = move_direction
        self.task_state = task_state


# 电梯进程类
class ElevatorThread(QThread):
    def __init__(self, elevator_index):
        """
        初始化电梯进程类

        :param elevator_index: 电梯索引
        :type elevator_index: int
        """
        super().__init__()
        self.elevator_index = elevator_index

    def floor_travel(self, move_direction):
        """
        电梯上下楼操作

        :param move_direction: 电梯运行方向
        :type move_direction: ElevatorState
        """
        if move_direction == ElevatorState.UPWARD:
            elevator_state[self.elevator_index] = ElevatorState.UPWARD
        elif move_direction == ElevatorState.DOWNWARD:
            elevator_state[self.elevator_index] = ElevatorState.DOWNWARD

        elapsed_time = 0
        while elapsed_time != FLOOR_TRAVEL_TIME:
            mutex.unlock()
            self.msleep(THREAD_INTERVAL)
            elapsed_time += THREAD_INTERVAL
            mutex.lock()
            if elevator_state[self.elevator_index] == ElevatorState.WARNING:
                self.warning_handler()
                return

        if move_direction == ElevatorState.UPWARD:
            elevator_current_floor[self.elevator_index] += ElevatorState.UPWARD.value
        elif move_direction == ElevatorState.DOWNWARD:
            elevator_current_floor[self.elevator_index] += ElevatorState.DOWNWARD.value
        elevator_state[self.elevator_index] = ElevatorState.STATIONARY

        if elevator_state[self.elevator_index] == ElevatorState.WARNING:
            self.warning_handler()

    def door_toggle(self):
        """
        电梯开关门操作
        """
        elapsed_time = 0
        elevator_state[self.elevator_index] = ElevatorState.DOOR_OPERATING
        while True:
            if elevator_state[self.elevator_index] == ElevatorState.WARNING:
                self.warning_handler()
                break
            elif elevator_state[self.elevator_index] == ElevatorState.DOOR_OPERATING:
                mutex.unlock()
                self.msleep(THREAD_INTERVAL)
                elapsed_time += THREAD_INTERVAL
                mutex.lock()
                elevator_door_operating_progress[self.elevator_index] = elapsed_time / DOOR_TOGGLE_TIME

            if elevator_door_operating_progress[self.elevator_index] == 1.0:
                elevator_state[self.elevator_index] = ElevatorState.STATIONARY
                elevator_door_operating_progress[self.elevator_index] = 0.0
                break

    def warning_handler(self):
        """
        电梯报警处理
        """
        elevator_state[self.elevator_index] = ElevatorState.WARNING
        elevator_door_operating_progress[self.elevator_index] = 0.0
        for elevator_task in elevator_task_list:
            if elevator_task.task_state == ElevatorTaskState.IN_PROGRESS:
                if (elevator_task.target_floor in elevator_up_queue[self.elevator_index]
                        or elevator_task.target_floor in elevator_down_queue[self.elevator_index]):
                    elevator_task.task_state = ElevatorTaskState.PENDING
        elevator_up_queue[self.elevator_index] = []
        elevator_down_queue[self.elevator_index] = []

    def run(self):
        """
        运行电梯线程
        """
        while True:
            mutex.lock()
            if elevator_state[self.elevator_index] == ElevatorState.WARNING:
                self.warning_handler()
                mutex.unlock()
                continue

            if move_directions[self.elevator_index] == ElevatorState.UPWARD:
                if elevator_up_queue[self.elevator_index]:
                    if elevator_up_queue[self.elevator_index][0] == elevator_current_floor[self.elevator_index]:
                        self.door_toggle()
                        for outer_task in elevator_task_list:
                            if outer_task.target_floor == elevator_current_floor[self.elevator_index]:
                                outer_task.task_state = ElevatorTaskState.COMPLETED
                                break
                        if elevator_up_queue:
                            elevator_up_queue[self.elevator_index].pop(0)
                    elif elevator_up_queue[self.elevator_index][0] > elevator_current_floor[self.elevator_index]:
                        self.floor_travel(ElevatorState.UPWARD)
                elif elevator_up_queue[self.elevator_index] == [] and elevator_down_queue[self.elevator_index] != []:
                    move_directions[self.elevator_index] = ElevatorState.DOWNWARD

            elif move_directions[self.elevator_index] == ElevatorState.DOWNWARD:
                if elevator_down_queue[self.elevator_index]:
                    if elevator_down_queue[self.elevator_index][0] == elevator_current_floor[self.elevator_index]:
                        self.door_toggle()
                        for outer_task in elevator_task_list:
                            if outer_task.target_floor == elevator_current_floor[self.elevator_index]:
                                outer_task.task_state = ElevatorTaskState.COMPLETED
                                break
                        if elevator_down_queue:
                            elevator_down_queue[self.elevator_index].pop(0)
                    elif elevator_down_queue[self.elevator_index][0] < elevator_current_floor[self.elevator_index]:
                        self.floor_travel(ElevatorState.DOWNWARD)
                elif elevator_down_queue[self.elevator_index] == [] and elevator_up_queue[self.elevator_index] != []:
                    move_directions[self.elevator_index] = ElevatorState.UPWARD

            mutex.unlock()


# 电梯控制类
class ElevatorController(QThread):
    def __init__(self):
        """
        初始化电梯控制类
        """
        super().__init__()

    @staticmethod
    def allocate_elevator(elevator_task):
        """
        为电梯任务分配电梯

        :param elevator_task: 电梯任务
        :type elevator_task: ElevatorTask
        :return: 电梯索引
        :rtype: int
        """
        min_distance = FLOOR_NUM + 1
        target_elevator_index = -1

        for elevator_index in range(ELEVATOR_NUM):
            if elevator_state[elevator_index] == ElevatorState.WARNING:
                continue

            target_indexes = []
            origin_index = elevator_current_floor[elevator_index]
            if elevator_state[elevator_index] == ElevatorState.UPWARD:
                origin_index += 1
            elif elevator_state[elevator_index] == ElevatorState.DOWNWARD:
                origin_index -= 1

            if move_directions[elevator_index] == ElevatorState.UPWARD:
                target_indexes = elevator_up_queue[elevator_index]
            elif move_directions[elevator_index] == ElevatorState.DOWNWARD:
                target_indexes = elevator_down_queue[elevator_index]

            if not target_indexes:
                distance = abs(origin_index - elevator_task.target_floor)
            elif move_directions[elevator_index] == elevator_task.move_direction and (
                    (elevator_task.move_direction == ElevatorState.UPWARD
                     and elevator_task.target_floor >= origin_index) or (
                            elevator_task.move_direction == ElevatorState.DOWNWARD
                            and elevator_task.target_floor <= origin_index)):
                distance = abs(origin_index - elevator_task.target_floor)
            else:
                distance = abs(origin_index - target_indexes[-1]) + abs(elevator_task.target_floor - target_indexes[-1])

            if distance < min_distance:
                min_distance = distance
                target_elevator_index = elevator_index

        return target_elevator_index

    @staticmethod
    def add_elevator_task(elevator_index, elevator_task, descending=False):
        """
        添加电梯任务

        :param elevator_index: 电梯索引
        :type elevator_index: int
        :param elevator_task: 电梯任务
        :type elevator_task: ElevatorTask
        :param descending: 降序 / 升序
        :type descending: bool
        """
        if descending:
            elevator_task_queue = elevator_down_queue[elevator_index]
        else:
            elevator_task_queue = elevator_up_queue[elevator_index]

        if elevator_task.target_floor not in elevator_task_queue:
            elevator_task_queue.append(elevator_task.target_floor)
            elevator_task_queue.sort(reverse=descending)
            elevator_task.task_state = ElevatorTaskState.IN_PROGRESS

    def run(self):
        """
        运行电梯控制进程
        """
        while True:
            global elevator_task_list

            mutex.lock()

            for outer_task in elevator_task_list:
                if outer_task.task_state == ElevatorTaskState.PENDING:
                    target_id = self.allocate_elevator(outer_task)
                    if ((elevator_current_floor[target_id] == outer_task.target_floor
                         and outer_task.move_direction == ElevatorState.UPWARD
                         and elevator_state[target_id] != ElevatorState.UPWARD)
                            or elevator_current_floor[target_id] < outer_task.target_floor):
                        self.add_elevator_task(target_id, outer_task)
                    elif ((elevator_current_floor[target_id] == outer_task.target_floor
                           and outer_task.move_direction == ElevatorState.DOWNWARD
                           and elevator_state[target_id] != ElevatorState.DOWNWARD)
                          or elevator_current_floor[target_id] > outer_task.target_floor):
                        self.add_elevator_task(target_id, outer_task, descending=True)

            elevator_task_list = [task for task in elevator_task_list if task.task_state != ElevatorTaskState.COMPLETED]

            mutex.unlock()


# 程序入口
if __name__ == '__main__':
    # 创建 Qt 应用程序实例
    app = QtWidgets.QApplication(sys.argv)

    # 初始化窗口类
    main_window = MainWindow()

    # 初始化并运行电梯控制类
    outer = ElevatorController()
    outer.start()
    main_window.console_output('电梯控制进程已运行')

    # 初始化并运行电梯进程类
    elevator_threads = [ElevatorThread(elevator_index) for elevator_index in range(ELEVATOR_NUM)]
    for elevator_index in range(ELEVATOR_NUM):
        elevator_threads[elevator_index].start()
        main_window.console_output(f'{elevator_index + 1}号电梯进程已运行')

    # 启动事件循环
    sys.exit(app.exec_())
