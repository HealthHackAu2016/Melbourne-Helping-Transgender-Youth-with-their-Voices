//This file sends the score when a request is made to the path http://localhost:3000/analyse
var bodyparser = require('body-parser');
var express = require('express');
var status = require('http-status');
var _ = require('underscore');
var exec = require('child_process').exec;
var R = require("r-script");
var app = express();

var cmd = 'Rscript fileR.R';
var cmd1 = 'Rscript mac_learn_prob.R';
var cmd2 = './testthis.sh';


app.get('/analyse', function (req, res) {
exec(cmd, function(error, stdout, stderr) {

exec(cmd1, function(error, stdout, stderr) {exec(cmd2, function(error, stdout, stderr) {
console.log(stdout)
//This is the response to the score request corresponding to a given voice
res.send(stdout);

  
});
  
});
  
});

  
});

app.listen(3000, function () {
  console.log('Example app listening on port 3000!');
});

  





 

