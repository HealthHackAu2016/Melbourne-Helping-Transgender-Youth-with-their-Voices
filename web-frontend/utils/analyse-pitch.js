// TODO
// - try out autocorrelation in frequency domain
// - put X frequencies in an array and return median
// - try using the location of the first peak as the difference

var worker = new Worker('/pitchWorker.js');
let currentCallback
worker.onmessage = (e) => {
    if (currentCallback) {
        currentCallback(e.data)
    }
}

module.exports = function analysePitch(input, sampleRate, callback) {
    currentCallback = callback
    worker.postMessage({
        buffer: input,
        sampleRate
    })
}
