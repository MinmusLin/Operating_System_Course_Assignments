// 内存块类
export class MemoryBlock {
    id: number  // 唯一标识符，用于识别每个内存块
    size: number  // 内存块的大小，以某种单位（如KB）计量
    isFree: boolean  // 布尔值，表示内存块是否为空闲状态

    constructor() {
        this.id = 0
        this.size = 0
        this.isFree = false
    }
}

// 内存管理类
export class MemoryManager {
    memoryBlocks: MemoryBlock[] = [{id: 1, size: 640, isFree: true}]  // 存放所有内存块的数组
    lastId: number = 1  // 记录最后一个被分配的内存块的ID，用于生成新内存块的唯一标识符

    firstFit(size: number): void {
        for (let block of this.memoryBlocks) {
            if (block.isFree && block.size >= size) {
                this.allocateMemory(block, size)
                return
            }
        }
    }

    bestFit(size: number): void {
        let bestBlock: MemoryBlock | null = null
        for (let block of this.memoryBlocks) {
            if (block.isFree && block.size >= size) {
                if (!bestBlock || block.size < bestBlock.size) {
                    bestBlock = block
                }
            }
        }
        if (bestBlock) {
            this.allocateMemory(bestBlock, size)
        }
    }

    allocateMemory(block: MemoryBlock, size: number): void {
        if (block.size === size) {
            block.isFree = false
        } else {
            const remainingSize = block.size - size
            block.size = size
            block.isFree = false
            this.memoryBlocks.push({id: ++this.lastId, size: remainingSize, isFree: true})
        }
    }

    releaseMemory(id: number): void {
        const blockIndex = this.memoryBlocks.findIndex(block => block.id === id && !block.isFree)
        if (blockIndex !== -1) {
            this.memoryBlocks[blockIndex].isFree = true
            this.mergeMemory()
        }
    }

    mergeMemory(): void {
        this.memoryBlocks.sort((a, b) => a.id - b.id)
        let blockIndex = 0
        while (blockIndex < this.memoryBlocks.length - 1) {
            if (this.memoryBlocks[blockIndex].isFree && this.memoryBlocks[blockIndex + 1].isFree) {
                this.memoryBlocks[blockIndex].size += this.memoryBlocks[blockIndex + 1].size
                this.memoryBlocks.splice(blockIndex + 1, 1)
            } else {
                blockIndex++
            }
        }
    }

    getMemoryBlocks(): MemoryBlock[] {
        return this.memoryBlocks
    }
}

// 页面类
export class Page {
    id: number  // 页面的编号，唯一标识一个页面
    instructions: number[]  // 存放在该页面上的指令集，通常为一个整数数组

    constructor(id: number) {
        this.id = id
        this.instructions = Array.from({length: 10}, (_, i) => id * 10 + i)
    }
}

// 内存类
export class Memory {
    size: number  // 内存中可以存放的页面数
    method: 'FIFO' | 'LRU'  // 页面置换算法，如 FIFO 或 LRU
    pages: Page[]  // 当前加载在内存中的页面数组
    lastUsedTime: number[]  // 对于 LRU 算法，记录每页的最后使用时间

    constructor(size: number, method: 'FIFO' | 'LRU') {
        this.size = size
        this.method = method
        this.pages = []
        this.lastUsedTime = Array(4).fill(0)
    }

    hasInstruction(instruction: number, time: number = 0): boolean {
        const pageIndex = this.pages.findIndex(page => page.instructions.includes(instruction))
        if (pageIndex !== -1) {
            if (this.method === 'LRU') {
                this.lastUsedTime[pageIndex] = time
            }
            return true
        }
        return false
    }
}

// 内存状态类
export class MemoryState {
    logs: {
        id: number  // 编号
        instructionId: number  // 指令代号
        pages: (string | number)[]  // 内存块
        isPageFault: boolean  // 缺页
        insertedBlock: string | number  // 放入
        removedPage: string | number  // 换出
    }[]  // 存放每次内存操作的详细日志

    constructor() {
        this.logs = []
    }

    addLog(id: number, instructionId: number, memory: Memory, isPageFault: boolean, insertedBlock: number | null, removedPage: number | null): void {
        const logEntry = {
            id: id,
            instructionId,
            pages: memory.pages.map(page => page ? page.id : '-'),
            isPageFault: isPageFault,
            insertedBlock: insertedBlock !== null ? insertedBlock + 1 : '-',
            removedPage: removedPage !== null ? removedPage : '-'
        }
        this.logs.push(logEntry)
    }
}
