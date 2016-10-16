
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

#Machine_learning_algorithms-based on work by Kory Becker
# This file contains the male and female training parameters for 3168 individuals
load("data.bin")
# Create a train and test set.
set.seed(777)
spl <- sample.split(data$label, 0.7)
train <- subset(data, spl == TRUE)
test <- subset(data, spl == FALSE)

# Build models.
genderLog <- glm(label ~ ., data=train, family='binomial')
genderCART <- rpart(label ~ ., data=train, method='class')
prp(genderCART)
genderForest <- randomForest(label ~ ., data=train)



# Accuracy: 0.72
predictLog <- predict(genderLog, type='response')
table(train$label, predictLog >= 0.5)

# Accuracy: 0.71
predictLog2 <- predict(genderLog, newdata=test, type='response')
table(test$label, predictLog2 >= 0.5)

# Accuracy: 0.81
predictCART <- predict(genderCART)
predictCART.prob <- predictCART[,2]
table(train$label, predictCART.prob >= 0.5)

# Accuracy: 0.78
predictCART2 <- predict(genderCART, newdata=test)
predictCART2.prob <- predictCART2[,2]
table(test$label, predictCART2.prob >= 0.5)


# Accuracy: 1
predictForest <- predict(genderForest, newdata=train)
table(train$label, predictForest)

# Accuracy: 0.86
predictForest <- predict(genderForest, newdata=test)
table(test$label, predictForest)


# Tune random-forest and return best model.
# Accuracy: 0.87
set.seed(777)
genderTunedForest <- tuneRF(train[, -21], train[, 21], stepFactor=.5, doBest=TRUE)
predictForest <- predict(genderTunedForest, newdata=test)
table(test$label, predictForest)


# Try svm (gamma and cost determined from tuning).
set.seed(777)
genderSvm <- svm(label ~ ., data=train, gamma=0.21, cost=8)

# Accuracy: 0.96
predictSvm <- predict(genderSvm, train)
table(predictSvm, train$label)


# Accuracy: 0.85
predictSvm <- predict(genderSvm, test)
table(predictSvm, test$label)




# Try XGBoost.
# Accuracy: 1
trainx <- sapply(train, as.numeric)
trainx[,21] <- trainx[,21] - 1
set.seed(777)
genderXG <- xgboost(data = trainx[,-21], label = trainx[,21], eta=0.2, nround = 500, subsample = 0.5, colsample_bytree = 0.5, objective = "binary:logistic")
results <- predict(genderXG, trainx)
table(trainx[,21], results >= 0.5)

# Accuracy: 0.87
testx <- sapply(test, as.numeric)
testx[,21] <- testx[,21] - 1
results <- predict(genderXG, testx)
table(testx[,21], results >= 0.5)
(414 + 413) / nrow(test)

# Try stacking models in an ensemble.
results1 <- predict(genderSvm, newdata=test)
results2 <- predict(genderTunedForest, newdata=test)
results3 <- factor(as.numeric(predict(genderXG, testx) >= 0.5), labels = c('male', 'female'))
combo <- data.frame(results1, results2, results3, y = test$label)

# Accuracy: 0.89
set.seed(777)
genderStacked <- tuneRF(combo[,-4], combo[,4], stepFactor=.5, doBest=TRUE)
predictStacked <- predict(genderStacked, newdata=combo)
table(predictStacked, test$label)

# Accuracy: 1
results1 <- predict(genderSvm, newdata=train)
results2 <- predict(genderTunedForest, newdata=train)
results3 <- factor(as.numeric(predict(genderXG, trainx) >= 0.5), labels = c('male', 'female'))
combo <- data.frame(results1, results2, results3)
predictStacked <- predict(genderStacked, newdata=combo)
table(predictStacked, train$label)

