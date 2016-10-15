module.exports = function smooth(length = 100) {
  const history = []
  let runningTotal = 0
  return (value) => {
    runningTotal += value
    history.push(value)
    if (history.length > length) {
      runningTotal -= history.shift()
    }
    return runningTotal / history.length
  }
}
