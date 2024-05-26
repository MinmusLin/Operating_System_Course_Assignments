<template>
  <h1 style='text-align: center'>Memory Management | 内存管理</h1>
  <h3 style='text-align: center'>2250758 林继申</h3>

  <div class='container'>
    <div class='half'>
      <h2>动态分区分配方式模拟</h2>
      <h3>Dynamic Partition Allocation Method Simulation</h3>

      <el-button-group>
        <el-button type='primary' plain size='large' round
                   @click="runPagingAllocationSimulation('firstFit')">
          模拟首次适应（First Fit）算法
        </el-button>
        <el-button type='primary' plain size='large' round
                   @click="runPagingAllocationSimulation('bestFit')">
          模拟最佳适应（Best Fit）算法
        </el-button>
        <el-button type='primary' plain size='large' round @click='restartMemoryManager'>重置</el-button>
      </el-button-group>

      <div style='display: flex; justify-content: space-around; width: 100%'>
        <el-table stripe :data='taskSequence' style='width: 360px'>
          <el-table-column prop='id' label='任务序号' width='120' align='center'/>
          <el-table-column prop='description' label='任务内容' width='240' align='center'/>
        </el-table>

        <el-table :data='memoryBlocks' stripe style='width: 360px'>
          <el-table-column prop='id' label='分区 ID' width='120' align='center'/>
          <el-table-column prop='size' label='大小 (K)' width='120' align='center'/>
          <el-table-column prop='isFree' label='是否空闲' width='120' align='center'>
            <!--suppress JSUnresolvedReference -->
            <template #default='scope'>
              {{ scope.row.isFree ? '是' : '否' }}
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>

    <div class='half'>
      <h2>请求分页分配方式模拟</h2>
      <h3>Demand Paging Allocation Method Simulation</h3>

      <el-button-group>
        <el-button type='primary' plain size='large' round @click='runFifoSimulation'>模拟先进先出（FIFO）算法</el-button>
        <el-button type='primary' plain size='large' round @click='runLruSimulation'>模拟最近最少使用（LRU）算法</el-button>
        <el-button type='primary' plain size='large' round @click='restartPartitionAllocationSimulation'>重置
        </el-button>
      </el-button-group>

      <div style='margin-top: 10px; margin-bottom: 10px'>
        <p>缺页数：{{ pageFaults }}</p>
        <p>缺页率：{{ (pageFaultRate * 100).toFixed(2) }}%</p>
        <p>执行时间：{{ executionTime }} 毫秒</p>
      </div>

      <el-table :data='memoryState' stripe style='width: 720px'>
        <el-table-column prop='id' label='编号' width='60' align='center'/>
        <el-table-column prop='instructionId' label='指令代号' width='100' align='center'/>
        <el-table-column label='内存块' width='320' align='center'>
          <el-table-column label='内存块 1' width='80' align='center'>
            <!--suppress JSUnresolvedReference -->
            <template #default='scope'>
              {{ scope.row.pages[0] ? scope.row.pages[0] : '-' }}
            </template>
          </el-table-column>
          <el-table-column label='内存块 2' width='80' align='center'>
            <!--suppress JSUnresolvedReference -->
            <template #default='scope'>
              {{ scope.row.pages[1] ? scope.row.pages[1] : '-' }}
            </template>
          </el-table-column>
          <el-table-column label='内存块 3' width='80' align='center'>
            <!--suppress JSUnresolvedReference -->
            <template #default='scope'>
              {{ scope.row.pages[2] ? scope.row.pages[2] : '-' }}
            </template>
          </el-table-column>
          <el-table-column label='内存块 4' width='80' align='center'>
            <!--suppress JSUnresolvedReference -->
            <template #default='scope'>
              {{ scope.row.pages[3] ? scope.row.pages[3] : '-' }}
            </template>
          </el-table-column>
        </el-table-column>
        <el-table-column prop='isPageFault' label='缺页' width='80' align='center'>
          <!--suppress JSUnresolvedReference -->
          <template #default='scope'>
            {{ scope.row.isPageFault ? '是' : '否' }}
          </template>
        </el-table-column>
        <el-table-column prop='insertedBlock' label='放入' width='80' align='center'/>
        <el-table-column prop='removedPage' label='换出' width='80' align='center'/>
      </el-table>
    </div>
  </div>
</template>

<script setup lang='ts'>
import {MemoryBlock, MemoryManager, MemoryState} from './classes'
import {fifoSimulation, lruSimulation} from './utils'
import {ref} from 'vue'
import {ElMessage} from 'element-plus'

// 动态分区分配方式模拟

const memoryManager = ref(new MemoryManager())
const memoryBlocks = ref<MemoryBlock[]>(memoryManager.value.getMemoryBlocks())
const currentTask = ref(0)

const taskSequence = ref([
  {id: 1, description: '作业 1 申请 130K'},
  {id: 2, description: '作业 2 申请 60K'},
  {id: 3, description: '作业 3 申请 100K'},
  {id: 4, description: '作业 2 释放 60K'},
  {id: 5, description: '作业 4 申请 200K'},
  {id: 6, description: '作业 3 释放 100K'},
  {id: 7, description: '作业 1 释放 130K'},
  {id: 8, description: '作业 5 申请 140K'},
  {id: 9, description: '作业 6 申请 60K'},
  {id: 10, description: '作业 7 申请 50K'},
  {id: 11, description: '作业 6 释放 60K'}
])

const firstFitSequence = [
  () => memoryManager.value.firstFit(130),
  () => memoryManager.value.firstFit(60),
  () => memoryManager.value.firstFit(100),
  () => memoryManager.value.releaseMemory(2),
  () => memoryManager.value.firstFit(200),
  () => memoryManager.value.releaseMemory(3),
  () => memoryManager.value.releaseMemory(1),
  () => memoryManager.value.firstFit(140),
  () => memoryManager.value.firstFit(60),
  () => memoryManager.value.firstFit(50),
  () => memoryManager.value.releaseMemory(6)
]

const bestFitSequence = [
  () => memoryManager.value.bestFit(130),
  () => memoryManager.value.bestFit(60),
  () => memoryManager.value.bestFit(100),
  () => memoryManager.value.releaseMemory(2),
  () => memoryManager.value.bestFit(200),
  () => memoryManager.value.releaseMemory(3),
  () => memoryManager.value.releaseMemory(1),
  () => memoryManager.value.bestFit(140),
  () => memoryManager.value.bestFit(60),
  () => memoryManager.value.bestFit(50),
  () => memoryManager.value.releaseMemory(6),
]

const runPagingAllocationSimulation = (algorithmType: string) => {
  let sequence: (() => void)[] = []
  let message: string = ''
  if (algorithmType === 'firstFit') {
    sequence = firstFitSequence
    message = '模拟首次适应算法：'
  } else if (algorithmType === 'bestFit') {
    sequence = bestFitSequence
    message = '模拟最佳适应算法：'
  }
  if (currentTask.value < sequence.length) {
    sequence[currentTask.value]()
    ElMessage({
      showClose: true,
      message: message + taskSequence.value[currentTask.value].description,
      type: 'success',
      duration: 2000
    })
    currentTask.value++
    memoryBlocks.value = memoryManager.value.getMemoryBlocks()
  } else {
    ElMessage({
      showClose: true,
      message: '动态分区分配方式模拟已完成',
      type: 'success',
      duration: 2000
    })
  }
}

function restartMemoryManager() {
  memoryManager.value = new MemoryManager()
  memoryBlocks.value = memoryManager.value.getMemoryBlocks()
  currentTask.value = 0
  ElMessage({
    showClose: true,
    message: '动态分区分配方式模拟已重置',
    type: 'warning',
    duration: 2000
  })
}

// 请求分页分配方式模拟

const memoryState = ref<MemoryState[]>([])
const pageFaults = ref(0)
const pageFaultRate = ref(0)
const executionTime = ref(0)

function runPartitionAllocationSimulation(simulationFunction: () => {
  pageFaultRate: number,
  logs: Array<any>,
  pageFaults: number
}) {
  const startTime = performance.now()
  const result = simulationFunction()
  const endTime = performance.now()
  memoryState.value = result.logs
  pageFaults.value = result.pageFaults
  pageFaultRate.value = result.pageFaultRate
  executionTime.value = Number((endTime - startTime).toFixed(2))
  ElMessage({
    showClose: true,
    message: `模拟 ${simulationFunction === fifoSimulation ? 'FIFO' : 'LRU'} 置换算法已完成`,
    type: 'success',
    duration: 2000
  })
}

function runFifoSimulation() {
  runPartitionAllocationSimulation(fifoSimulation)
}

function runLruSimulation() {
  runPartitionAllocationSimulation(lruSimulation)
}

function restartPartitionAllocationSimulation() {
  memoryState.value = []
  pageFaults.value = 0
  pageFaultRate.value = 0
  executionTime.value = 0
  ElMessage({
    showClose: true,
    message: '请求分页分配方式模拟已重置',
    type: 'warning',
    duration: 2000
  })
}
</script>

<style>
.container {
  display: flex;
  width: 100%;
}

.half {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 50%;
}
</style>
