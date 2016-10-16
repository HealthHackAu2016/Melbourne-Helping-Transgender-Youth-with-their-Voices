import React from 'react'
import Layout from '../../components/Layout/Layout'
import styles from './styles.css';
import AudioSensor from '../../utils/audio-sensor'
import Visualizer from '../../components/visualizer'
import history from '../../core/history'

const MIN_PITCH = 85
const MAX_PITCH = 255
const SAMPLE_SIZE = 12
const MIN_AMPLITUDE = 0.001

class Calibration extends React.Component {
    constructor() {
        super()
        this.state = {
            status: 'ready',
            dataPoints: []
        }
    }

    render() {
        const { status, data, loudEnough, averagePitch, targetPitch } = this.state
        return (
            <Layout>
                <div className={styles.wrapper}>
                    <div>
                        <div className={styles.title}>
                            Calibration
                        </div>
                        {status !== 'done' && <div className={styles.paragraph}>
                            Click the Start button, then read the following text:
                        </div>}
                        {status !== 'done' && <div className={styles.exampleText}>
                            You wish to know all about my
                            grandfather. Well, he is nearly 93
                            years old, yet he still thinks as swiftly
                            as ever. He dresses himself in an old
                            black frock coat, usually several
                            buttons missing. A long beard clings
                            to his chin, giving those who observe
                            him a pronounced feeling of the
                            utmost respect.
                        </div>}
                        {status === 'done' && <div>
                            <div className={styles.paragraph}>Your voice: {averagePitch}hz</div>
                            <div className={styles.paragraph}>Set your pitch goal:</div>
                            <div className={styles.paragraph}><input type="range" min={averagePitch - 50} max={averagePitch + 50} onChange={this.changeTargetPitch.bind(this)} />
                                <span className={styles.targetPitch}>{targetPitch}hz</span>
                                <span className={styles.pitchDiff}>{this.pitchDiff}</span>
                            </div>
                            <div className={styles.paragraph}>
                                The voiced speech of a typical adult male will have a fundamental frequency from 85 to 180 Hz, and that of a typical adult female from 165 to 255 Hz.
                            </div>
                        </div>}
                    </div>
                    <div>
                        {status === 'calibrating' && <Visualizer data={data} loudEnough={loudEnough} />}
                        <div className={styles.actions}>
                            {status === 'ready' && <button className={styles.action} onClick={this.start.bind(this)}>Start</button>}
                            {status === 'calibrating' && <span className={styles.talkPrompt}>Keep talking...</span>}
                            {status === 'done' && <button className={styles.action} onClick={this.continue.bind(this)}>Continue</button>}
                        </div>
                    </div>
                </div>
            </Layout>
        )
    }

    start() {
        this.setState({ status: 'calibrating', dataPoints: [] })
        if (!this.audioSensor) {
            this.audioSensor = new AudioSensor()
            this.audioSensor.onanalyse = this.onanalyse.bind(this)
        }
        this.audioSensor.start()
    }

    onanalyse({ data, amplitude, pitch }) {
        if (this.state.status === 'done') {
            return
        }
        const loudEnough = amplitude > MIN_AMPLITUDE
        const newState = {
            data,
            loudEnough,
        }
        if (loudEnough && pitch > MIN_PITCH && pitch < MAX_PITCH) {
            newState.dataPoints = this.state.dataPoints.concat([{
                amplitude, pitch
            }])
            if (newState.dataPoints.length >= SAMPLE_SIZE) {
                newState.status = 'done'
                this.audioSensor.stop()
                this.calcAveragePitch(newState.dataPoints)
                console.log(newState.dataPoints)
            }
        }
        this.setState(newState)
    }

    calcAveragePitch(dataPoints) {
        let totalPitch = 0
        dataPoints.forEach(({ pitch }) => {
            totalPitch += pitch
        })
        const averagePitch = Math.round(totalPitch / dataPoints.length)
        this.setState({ averagePitch, targetPitch: averagePitch })
    }

    changeTargetPitch(e) {
        this.setState({
            targetPitch: e.target.value
        })
    }

    get pitchDiff() {
        const diff = this.state.targetPitch - this.state.averagePitch
        if (diff > 0) {
            return `+${diff}hz`
        } else if (diff < 0) {
            return `-${-diff}hz`
        } else {
            return ''
        }
    }

    continue() {
        sessionStorage.setItem('pitch', this.state.averagePitch)
        sessionStorage.setItem('target', this.state.targetPitch)
        history.push('/play');
    }
}


export default Calibration
