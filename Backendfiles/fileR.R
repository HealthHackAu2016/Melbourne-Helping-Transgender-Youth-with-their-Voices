rm(list=ls())
library(magrittr)
library(tuneR)
library(seewave)
library(caTools)
library(rpart)
library(rpart.plot)

library(warbleR)
library(mice)
library(xgboost)
library(e1071)
# This file contains specan3 function that calculates the parameters for estimating the score from the sound
source('/home/bipin/Documents/voice-gender-master/gender_new.R')
data <- data.frame()

# This is the file with the sound of the person that needs to be transmitted to the server
list <- 'apba1.wav'
for (fileName in list) {
  row <- data.frame(fileName, 0, 0, 20);
  data <-rbind(data, row)
}

colnames(data)[which(names(data) == "fileName")] <- "sound.files"
colnames(data)[which(names(data) == "X0")] <- "selec"
colnames(data)[which(names(data) == "X0.1")] <- "start"
colnames(data)[which(names(data) == "X20")] <- "end"

result <- specan3(data, parallel=1)

acoustics <- result$acoustics

acoustics[,1] <- NULL
acoustics[,1] <- NULL
acoustics[,1] <- NULL
acoustics[,'peakf'] <- NULL
acoustics <- as.matrix(acoustics)
# This is the file with parameters for calculating the score
save(acoustics,file='/home/bipin/Documents/voice-gender-master/chapter3/acoustics.Rdata')



