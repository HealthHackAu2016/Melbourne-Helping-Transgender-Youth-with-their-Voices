# Machine learning backend using an example algorithm: logistic regression. 
library(tuneR)
library(seewave)
library(caTools)
library(rpart)
library(rpart.plot)
library(randomForest)
library(warbleR)
library(mice)
library(xgboost)
library(e1071)

# Change this to path for with training parameters that have been collected for men and women. This file is provided in the repository
load('/home/bipin/Documents/voice-gender-master/data.bin')
# Change this to path for file with the paramters from input voice file predicting probability (score) for the person. This file is provided in the repository
load(file='/home/bipin/Documents/voice-gender-master/chapter3/acoustics.Rdata')
# This bit creates the training model genderLog. You could save genderLog as Rdata and not repeat this everytime you predict probability for the person
set.seed(777)tr
# Use 1 instead of 0.7 if you would like to use data for everyone
spl <- sample.split(data$label, 0.7)
train <- subset(data, spl == TRUE)

test <-data.frame(acoustics)
test["label"]='male'


# Build models.
genderLog <- glm(label ~ ., data=train, family='binomial')
# Predict probability. Logistic regression is just an example. Other algorithms could be used
predictLog2 <- predict(genderLog, newdata=test, type='response')
probval=matrix(predictLog2[1])
# Predicted score is stored by this line
write.csv(probval[1,1],file='/home/bipin/Documents/voice-gender-master/chapter3/logval.csv')
