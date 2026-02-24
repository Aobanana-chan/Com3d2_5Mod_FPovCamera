const UpdateQueue: Set<() => void> = new Set()
const LateUpdateQueue: Set<() => void> = new Set()

export function addUpdate(func: () => void) {
  UpdateQueue.add(func)
}

export function addLateUpdate(func: () => void) {
  LateUpdateQueue.add(func)
}

export function removeUpdate(func: () => void): boolean {
    return UpdateQueue.delete(func)
}

export function removeLateUpdate(func: () => void): boolean {
    return LateUpdateQueue.delete(func)
}

export function clearUpdate() {
  UpdateQueue.clear()
}

export function clearLateUpdate() {
    LateUpdateQueue.clear()
}

export function update(){
    for(const func of UpdateQueue){
        func()
    }
}

export function lateUpdate(){
    for(const func of LateUpdateQueue){
        func()
    }
}