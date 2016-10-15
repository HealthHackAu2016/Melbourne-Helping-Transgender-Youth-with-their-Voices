
this.onmessage = function(e){
  var pitch = analysePitch(e.data.buffer, e.data.sampleRate)
  this.postMessage(pitch)
};

function analysePitch(input, sampleRate) {
    var buffer = new Float32Array(input.length),
        locations, diffs;

    // Perform autocorrelation on input signal
    xcorr(input, buffer);

    // Find peaks
    locations = findpeaks(buffer);

    // Calculate frequency from median value of distances between peaks
    diffs = diff(locations);
    freq = sampleRate / median(diffs);

    return freq
};

/// Lame MATLAB/Octave-like functions
function xcorr(input, output) {
    var n = input.length,
        norm, sum,  i, j;

    for (i = 0; i < n; i++) {
        sum = 0;
        for (j = 0; j < n; j++) {
            sum += (input[j] * (input[j+i] || 0)); // Pad input with zeroes
        }
        if (i === 0) norm = sum;
        output[i] = sum / norm;
    }
}

function findpeaks(data) {
    var locations = [0];

    for (var i = 1; i < data.length - 1; i++) {
        if (data[i] > 0 && data[i-1] < data[i] && data[i] > data[i+1]) {
            locations.push(i);
        }
    }

    return locations;
}

function diff(data) {
    return data.reduce(function (acc, value, i) {
        acc[i] = data[i] - data[i-1]; return acc;
    }, []).slice(1);
}

function mean(data) {
    return data.reduce(function (acc, value) {
        return acc + value;
    }, 0) / data.length;
}

function median(data) {
    var half;

    data.sort( function(a, b) {return a - b;} );
    half = Math.floor(data.length/2);

    if (data.length % 2)
        return data[half];
    else
        return (data[half-1] + data[half]) / 2.0;
}
