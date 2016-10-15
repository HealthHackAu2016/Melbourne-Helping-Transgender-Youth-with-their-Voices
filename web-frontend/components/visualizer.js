import React from 'react'
import styles from './visualizer.css'

export default class Visualizer extends React.Component {
	componentWillReceiveProps({ data, loudEnough }) {
		this.draw(data, loudEnough)
	}

	render() {
		return (<canvas className={styles.canvas} ref='canvas' />)
	}

	draw(buffer, loudEnough) {
		if (!this.refs.canvas) {
			return
		}
		const canvas = this.refs.canvas
		const context = this.refs.canvas.getContext('2d')
		const { width, height } = canvas

	  	context.fillStyle = '#333'
		context.fillRect(0, 0, width, height)

		context.lineWidth = 2
		context.strokeStyle = loudEnough ? '#0c0' : '#c00'

		context.beginPath()

		var sliceWidth = width * 1.0 / buffer.length;
		var x = 0;

		for(var i = 0; i < buffer.length; i++) {

			var v = (buffer[i] + 1)
			var y = v * height / 2

			if (i === 0) {
				context.moveTo(x, y)
			} else {
				context.lineTo(x, y)
			}

			x += sliceWidth
		}

		context.lineTo(width, canvas.height/2);
		context.stroke();
	}l
}
