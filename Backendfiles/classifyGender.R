# This code has been adapted from the code by Kory Becker available at: 
# https://github.com/primaryobjects/voice-gender
#
# Article accompanying the above code:
# http://www.primaryobjects.com/2016/06/22/identifying-the-gender-of-a-voice-using-machine-learning/
#
# Developed as part the TransVoice team at HealthHack 2016, Melbourne (15-16 October)
#

# Clear workspace and environment
rm(list=ls()) 

# Set working directory - PLEASE UPDATE FOR YOUR IMPLEMENTATION!
setwd("~/Documents/HealthHack/")

# Use original code from Kory Becker available at: https://github.com/primaryobjects/voice-gender
source('genderData/gender.R')

# Start with empty data.frame.
data <- data.frame()

# Get list of files in the working directory. Beware of case-sensitive file extensions i.e., '.WAV'!
list <- list.files(getwd(), '\\.wav')

# Add file list to data.frame for processing.
for (fileName in list) {
  row <- data.frame(fileName, 0, 0, 20)
  data <- rbind(data, row)
}
# Set column names for data frame
names(data) <- c('sound.files', 'selec', 'start', 'end')

# Process audio files to extract acoustic features
result <- specan3(data, parallel=1)
acoustics <- result$acoustics
acoustics[,1:3] <- NULL
acoustics[,'peakf'] <- NULL
wave <- result$wave
acoustics <- as.matrix(acoustics)

# Load trained model (binary files)
model = 4 # Choose model (2, 3, 4 or 5 - the SVM model 1 is not used)
if (model == 2) {
  print('Using model: XGBoost Small')
  if (!exists('genderXG')) {
    load('genderData/xgboostSmall.bin')
  }
  
  fit <- genderXG
}
if (model == 3) {
  print('Using model: Tuned Random Forest')
  if (!exists('genderTunedForest')) {
    load('genderData/tunedForest.bin')
  }
  
  fit <- genderTunedForest
}
if (model == 4) {
  print('Using model: XGBoost Large')
  if (!exists('genderXG2')) {
    load('genderData/xgboostLarge.bin')
  }
  
  fit <- genderXG2
}
if (model == 5) {
  print('Using model: Stacked ensemble')
  if (!exists('genderStacked')) {
    load('genderData/stacked.bin')
  }
  if (!exists('genderTunedForest')) {
    load('genderData/tunedForest.bin')
  }
  if (!exists('genderXG2')) {
    load('genderData/xgboostLarge.bin')
  }
  if (!exists('genderSvm')) {
    load('genderData/svm.bin')
  }
  
  fit <- genderStacked

  results1 <- predict(genderSvm, newdata=acoustics)
  results2 <- predict(genderTunedForest, newdata=acoustics)
  
  mf <- as.factor(c('male', 'female'))
  prob <- predict(genderXG2, newdata=acoustics)
  if (prob >= 0.5) {
    results3 <- mf[2]
  }
  else {
    results3 <- mf[1]
  }
  
  combo <- data.frame(results1, results2, results3)
  
  prob <- predict(genderStacked, newdata=combo, type='prob')[,2]
  result <- predict(genderStacked, newdata=combo)
}

# Predict gender from input audio using trained models
if (model != 5) {
  result <- predict(fit, newdata=acoustics)
  print(result)
}
if (model == 3) {
  prob <- predict(fit, newdata=acoustics, type='prob')[,2]
  print(prob)
}
if (model == 2 || model == 4) {
  prob <- result
  mf <- as.factor(c('male', 'female'))
  if (prob >= 0.5) {
    result <- mf[2]
  }
  else {
    result <- mf[1]
  }
}

# Display classification results
print(prob)
print(result)