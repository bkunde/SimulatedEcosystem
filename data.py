#!/bin/python

"""
           TIME 
            0         30   ... MAX_TIME
Rabbits:   AMOUNTOFRABBITS
Foxes:     AMOUNTOFFOXES23
"""


def main():
    lines = []
    file = open('data.txt', 'r')
    lines = file.readlines()
    START_RABBITS = 0
    START_FOXES = 0
    TIME_STEP = 10
    col = []
    timeCol = "Time, "
    rabbitCol = "Rabbits, "
    foxCol = "Foxes, "
    MAX_TIME = 0.0
    for line in lines:
        if line != '\n':
            line = line.strip()
            line = line.split(',')
            if ((float(line[2])) > MAX_TIME):
                MAX_TIME = float(line[2])

            if (line[2] == '0.000'):
                if (line[0] == 'Rabbit'):
                    START_RABBITS += 1
                elif (line[0] == 'Fox'):
                    START_FOXES += 1
            
    timeCol += '0'+ ','
    time = 0
    while (time < MAX_TIME):
        time += TIME_STEP
        timeCol += (str(int(time))+',')

    RABBITS = START_RABBITS
    FOXES = START_FOXES
    rabbitCol += str(RABBITS) + ', '
    foxCol += str(FOXES) + ', '
    time = timeCol.strip().split(',')
    i = 1
    for line in lines:
        if line == '\n':
            continue
        line = line.strip()
        linelist = line.split(',')
        if (linelist[2] != '0.000'):
            timestamp = str(round(float(linelist[2])))
            if (timestamp < time[i]):
                if (linelist[1] != 'born'):
                    if (linelist[0] == 'Rabbit'):
                        RABBITS -= 1
                    elif (linelist[0] == 'Fox'):
                        FOXES -= 1
                else:
                    if (linelist[0] == 'Rabbit'):
                        RABBITS += 1
                    elif (linelist[0] == 'Fox'):
                        FOXES += 1
            else:
                rabbitCol += str(RABBITS) + ', '
                foxCol += str(FOXES) + ', '
                if (linelist[1] != 'born'):
                    if (linelist[0] == 'Rabbit'):
                        RABBITS -= 1
                    elif (linelist[0] == 'Fox'):
                        FOXES -= 1
                else:
                    if (linelist[0] == 'Rabbit'):
                        RABBITS += 1
                    elif (linelist[0] == 'Fox'):
                        FOXES += 1
                i+=1
        
    #foxCol += str(FOXES) + ', '
                

    file.close()
    print(timeCol)
    print(rabbitCol)
    print(foxCol)
    

if __name__ == '__main__':
    main();
    
