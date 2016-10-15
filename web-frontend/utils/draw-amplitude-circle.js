
module.exports = function drawAmplitudeCircle(canvas, amplitude, pitch) {
    if (!canvas) {
      return
    }

    const canvasCtx = canvas.getContext('2d')
    const WIDTH = canvas.clientWidth
    const HEIGHT = canvas.clientHeight

    canvasCtx.fillStyle = 'rgb(200, 200, 200)';
    canvasCtx.fillRect(0, 0, WIDTH, HEIGHT);

    canvasCtx.lineWidth = 2;
    canvasCtx.strokeStyle = 'rgb(0, 0, 0)';

    const r = amplitude * 600
    if (r < 20) pitch = 0.5
    const x = pitch * WIDTH
    canvasCtx.beginPath()
    canvasCtx.ellipse(x, HEIGHT / 2, r, r, 0, 0, Math.PI * 2)
    canvasCtx.stroke()
}
