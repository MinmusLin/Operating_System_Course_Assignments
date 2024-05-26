import {Page, Memory, MemoryState} from './classes'

// 生成指令序列
function generateInstructionSequence() {
    let executionOrder = []
    let count = 0
    while (count < 320) {
        let m = Math.floor(Math.random() * 320)
        executionOrder.push(m)
        count++
        if (m + 1 < 320) {
            executionOrder.push(m + 1)
            count++
        }
        if (m > 0 && count < 320) {
            let m1 = Math.floor(Math.random() * m)
            executionOrder.push(m1)
            count++
            if (m1 + 1 < m) {
                executionOrder.push(m1 + 1)
                count++
            }
        }
        if (m + 2 < 320 && count < 320) {
            let m2 = m + 2 + Math.floor(Math.random() * (318 - m))
            executionOrder.push(m2)
            count++
            if (m2 + 1 < 320) {
                executionOrder.push(m2 + 1)
                count++
            }
        }
    }
    if (executionOrder.length > 320) {
        executionOrder.pop()
    }
    return executionOrder
}

// 模拟 FIFO 置换算法
export function fifoSimulation() {
    const log = new MemoryState()
    let pages = Array.from({length: 32}, (_, i) => new Page(i))
    let memory = new Memory(4, 'FIFO')
    let instructions = generateInstructionSequence()
    let pageFaults = 0
    let index = 0
    let count = 0
    for (let i of instructions) {
        count++
        let pageId = Math.floor(i / 10)
        if (!memory.hasInstruction(i)) {
            let removedPage = memory.pages.length >= memory.size ? memory.pages[index].id : null
            memory.pages[index] = pages[pageId]
            memory.lastUsedTime[index] = count
            pageFaults++
            log.addLog(count, i, memory, true, index, removedPage)
            index = (index + 1) % 4
        } else {
            log.addLog(count, i, memory, false, null, null)
        }
    }
    return {pageFaults: pageFaults, pageFaultRate: pageFaults / 320, logs: log.logs}
}

// 模拟 LRU 置换算法
export function lruSimulation() {
    const log = new MemoryState()
    let pages = Array.from({length: 32}, (_, i) => new Page(i))
    let memory = new Memory(4, 'LRU')
    let instructions = generateInstructionSequence()
    let pageFaults = 0
    let count = 0
    let index = 0
    for (let i of instructions) {
        count++
        let pageId = Math.floor(i / 10)
        if (!memory.hasInstruction(i, count)) {
            index = memory.pages.findIndex(page => page === undefined)
            if (index === -1) {
                index = memory.lastUsedTime.indexOf(Math.min(...memory.lastUsedTime))
            }
            let removedPage = memory.pages[index] ? memory.pages[index].id : null
            memory.pages[index] = pages[pageId]
            memory.lastUsedTime[index] = count
            pageFaults++
            log.addLog(count, i, memory, true, index, removedPage)
        } else {
            log.addLog(count, i, memory, false, null, null)
        }
    }
    return {pageFaults: pageFaults, pageFaultRate: pageFaults / 320, logs: log.logs}
}
