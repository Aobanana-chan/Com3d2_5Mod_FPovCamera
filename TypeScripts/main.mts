import { addLateUpdate, addUpdate, clearLateUpdate, clearUpdate, removeLateUpdate } from "./ticker.mjs"
import { clamp } from "./utils.mjs"

const Vector3 = CS.UnityEngine.Vector3
const UnityEngine = CS.UnityEngine

let plugin = CS.UnityEngine.GameObject.FindObjectOfType(puer.$typeof(CS.Com3d2Mod.Plugin)) as CS.Com3d2Mod.Plugin

class PovController {
    isInPovMode: boolean = false
    posOffset: CS.UnityEngine.Vector3 = Vector3.zero

    movementSpeed: number = 1  // TODO: 可以和ConfigureManager配合自定义
    rotationSpeed: number = 2  // TODO: 可以和ConfigureManager配合自定义

    enableMouseControl: boolean = false

    maximumVerticalAngle: number = 60
    minimumVerticalAngle: number = -60

    rotationY: number = 0
    rotationX: number = 0
    constructor() {

    }
    public onTriggerPov() {
        console.log("切换视角")
        if (CS.YotogiManager.instans == null) {
            console.warn("当前不在游艺场景，无法切换视角")
            this.isInPovMode = false
            return
        }
        let currentMaid = this.getCurrentMaid()
        if (currentMaid != null) {
            if (this.isInPovMode) {
                this.exitPovMode()
            } else {
                this.enterPovMode(currentMaid)
            }
        }
    }

    private getCurrentMaid(): CS.Maid | null {
        try {
            let yotogiManager = CS.YotogiManager.instans
            if (yotogiManager != null) {
                return yotogiManager.maid
            }
        }
        catch (error) {
            console.error("获取当前Maid失败", error)
        }
        return null
    }

    private enterPovMode(maid: CS.Maid) {
        this.isInPovMode = true
        CS.GameMain.Instance.MainCamera.SetControl(false)
        let headTrans = maid.body0.trsHead
        CS.GameMain.Instance.MainCamera.SetCameraType(CS.CameraMain.CameraType.Free)
        CS.GameMain.Instance.MainCamera.transform.rotation = UnityEngine.Quaternion.LookRotation(headTrans.up, Vector3.op_Multiply(headTrans.right, -1))
        this.posOffset = Vector3.op_Multiply(Vector3.up, 0.1)
        // this.posOffset = Vector3.zero
        this.rotationY = 0
        this.rotationX = 0
    }

    private handleRotation() {
        let mouseX = CS.UnityEngine.Input.GetAxis("Mouse X") * this.rotationSpeed
        let mouseY = CS.UnityEngine.Input.GetAxis("Mouse Y") * this.rotationSpeed
        this.rotationY += mouseY
        this.rotationX += mouseX
        this.rotationY = clamp(this.rotationY, this.minimumVerticalAngle, this.maximumVerticalAngle)
    }

    private handleMovement(trans: CS.UnityEngine.Transform) {
        let cameraTrans = CS.GameMain.Instance.MainCamera.transform
        let horizontal = ((CS.UnityEngine.Input.GetKey(UnityEngine.KeyCode.W) ? 1 : 0) - (CS.UnityEngine.Input.GetKey(UnityEngine.KeyCode.S) ? 1 : 0)) * this.movementSpeed * UnityEngine.Time.deltaTime
        let vertical = ((CS.UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) ? 1 : 0) - (CS.UnityEngine.Input.GetKey(UnityEngine.KeyCode.A) ? 1 : 0)) * this.movementSpeed * UnityEngine.Time.deltaTime
        if (horizontal != 0 || vertical != 0) {
            // COM3D2的坐标系和Unity是反的
            // let forward = Vector3.op_Addition(Vector3.op_Multiply(cameraTrans.forward, vertical), Vector3.op_Multiply(cameraTrans.right, horizontal))
            let forward = Vector3.op_Addition(Vector3.op_Multiply(cameraTrans.forward, horizontal), Vector3.op_Multiply(cameraTrans.right, vertical))
            cameraTrans.position = Vector3.op_Addition(cameraTrans.position, forward)
            this.posOffset = trans.InverseTransformPoint(cameraTrans.position)
        }
    }

    private setCursorState(visible: boolean) {
        UnityEngine.Cursor.lockState = visible ? UnityEngine.CursorLockMode.None : UnityEngine.CursorLockMode.Locked
        UnityEngine.Cursor.visible = visible
    }

    private setPosToHead(trans: CS.UnityEngine.Transform) {
        CS.GameMain.Instance.MainCamera.SetPos(trans.TransformPoint(this.posOffset))
    }

    private exitPovMode() {
        this.isInPovMode = false
        CS.GameMain.Instance.MainCamera.SetControl(true)
        CS.GameMain.Instance.MainCamera.SetCameraType(CS.CameraMain.CameraType.Target)
        this.setCursorState(true)
    }

    public onWorldReset() {
        this.isInPovMode = false
        this.setCursorState(true)
        CS.GameMain.Instance.MainCamera.SetControl(true)
        CS.GameMain.Instance.MainCamera.SetCameraType(CS.CameraMain.CameraType.Target)
    }

    public update() {
        if (!this.isInPovMode) {
            return
        }
        if (UnityEngine.Input.GetMouseButtonDown(1)) {
            this.enableMouseControl = !this.enableMouseControl
            this.setCursorState(!this.enableMouseControl)
        }
    }

    public lateUpdate() {
        if (!this.isInPovMode) {
            return
        }
        let headTrans = this.getCurrentMaid()!.body0.trsHead
        if (this.enableMouseControl) {
            this.handleRotation()
            this.handleMovement(headTrans)
        }

        let newRotation = UnityEngine.Quaternion.LookRotation(headTrans.up, Vector3.op_Multiply(headTrans.right, -1))
        CS.GameMain.Instance.MainCamera.transform.rotation = UnityEngine.Quaternion.op_Multiply(newRotation, UnityEngine.Quaternion.Euler(-this.rotationY, this.rotationX, 0))
        this.setPosToHead(headTrans)
    }
}

export function main() {
    if (plugin != null) {
        let povController = new PovController()
        plugin.onTriggerPov = povController.onTriggerPov.bind(povController)
        CS.Com3d2Mod.Plugin.onWorldReset = povController.onWorldReset.bind(povController)
        clearUpdate()
        clearLateUpdate()
        addLateUpdate(povController.lateUpdate.bind(povController))
        addUpdate(povController.update.bind(povController))
    }
    else {
        console.error("获取Plugin实例失败")
    }
}