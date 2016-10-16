const Recorder  = require('./recorder')
const analysePitch = require('./analyse-pitch')

const FAKE_INPUT = false

function analyseAmplitude(buffer) {
    let total = 0
    buffer.forEach(value => {
      total += value * value
    })
    return total / buffer.length
}

function scale(value, min, max) {
  const spread = max - min
  let out = (value - min) / spread
  if (out < 0) out = 0
  if (out > 1) out = 1
  return out
}

module.exports = class AudioSensor {
    constructor() {
        this.context = new AudioContext()
        this.analyser = this.context.createAnalyser()
    }

    start() {
        if (FAKE_INPUT) {
            this.osc = this.context.createOscillator();
            this.osc.frequency.value = 480
            this.osc.start()
            this.amp = this.context.createGain()
            this.amp.gain.value = 0.5
            this.osc.connect(this.amp)
            this.amp.connect(this.analyser)
        } else {
            var p = navigator.mediaDevices.getUserMedia({ audio: true, video: false })
            p.then((mediaStream) => {
                this.mediaStream = mediaStream
                const node = this.context.createMediaStreamSource(mediaStream)
                node.connect(this.analyser)
                this.recorder = new Recorder(this.analyser)
            }).catch((err) => { console.log(err.name); })
        }
        this.interval = setInterval(this.analyze.bind(this), 100)
    }

    stop() {
        if (this.interval !== undefined) {
            clearInterval(this.interval)
            this.interval = undefined
        }
        if (this.mediaStream) {
            const track = this.mediaStream.getTracks()[0]
            track.stop()
            this.mediaStream = undefined
        }
    }

    analyze() {
        if (!this.analyser) {
            return
        }
        if (this.osc) {
            this.osc.frequency.value += 5
            if (this.osc.frequency.value > 600) {
                this.osc.frequency.value = 50
            }
        }
        const data = new Float32Array(this.analyser.fftSize)
        this.analyser.getFloatTimeDomainData(data)
        analysePitch(data, this.context.sampleRate, pitch => {
            const amplitude = analyseAmplitude(data)
            if (this.onanalyse) {
                this.onanalyse({
                    data,
                    amplitude,
                    pitch
                })
            }
        })
    }

    startRecording() {
        if (!this.recorder) {
            console.error('no recorder')
            return
        }
        this.recorder.clear()
        this.recorder.record()
        this.recording = true
    }

    stopRecording() {
        this.recording = false
        this.recorder.stop()
        console.log('done', this.recorder)
        this.recorder.exportMonoWAV((blob) => {
            console.log('exported wav')
            const url = URL.createObjectURL(blob)
            const a = document.createElement('a')
            a.href = url
            a.download = 'recorded.wav'
            a.click()
            URL.revokeObjectURL(url)
        })
    }
}

