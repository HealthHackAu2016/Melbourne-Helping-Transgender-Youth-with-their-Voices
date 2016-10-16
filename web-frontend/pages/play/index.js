import React from 'react'
import AudioSensor from '../../utils/audio-sensor'

const MIN_AMPLITUDE = 0.001
const PITCH_RANGE = 100

export default class Play extends React.Component {
    constructor() {
        super()
        this.recording = false
        this.result = undefined
        this.pitch = undefined
        this.audioSensor = new AudioSensor()
        this.audioSensor.onanalyse = this.onanalyse.bind(this)
        this.target = parseInt(sessionStorage.getItem('target'), 10)
    }

    componentDidMount() {
        // Returns goal pitch (in hertz)
        window.GetTarget = () => this.target

        // Records a speech sample
        window.StartAttempt = this.startRecording.bind(this)

        // Checks if result is available (poll after StartAttempt)
        window.CheckResult = () => this.result

        // Returns pitch of current speech (if available)
        window.CheckPitch = () => this.pitch

        this.audioSensor.start()
    }

    componentWillUnmount() {
        this.audioSensor.stop()
        window.StartAttempt = () => undefined
        window.CheckResult = () => undefined
        window.CheckPitch = () => undefined
    }l

    startRecording() {
        this.recording = true
        setTimeout(this.finishRecording.bind(this), 2000)
        this.audioSensor.startRecording()
    }

    finishRecording() {
        this.recording = false
        this.audioSensor.stopRecording()
    }

    onanalyse({ pitch, amplitude }) {
        const MIN_PITCH = this.pitch - PITCH_RANGE
        const MAX_PITCH = this.pitch + PITCH_RANGE
        this.pitch = (amplitude > MIN_AMPLITUDE && pitch > MIN_PITCH && pitch < MAX_PITCH)
            ? pitch : undefined
    }

    render() {
        return (
            <div>game goes here</div>
        )
    }
}
