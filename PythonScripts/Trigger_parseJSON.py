from functools import reduce
from io import TextIOWrapper
import json
import os.path as path
import re
from typing import Callable, Dict, List, Tuple


def dataFile(filePath: str): return path.join("yelp_data",   filePath)
def outputFile(filePath: str): return path.join("yelp_output", filePath)


def cleanStr4SQL(s):
    return s.replace("'", "`").replace("\n", " ")


def parseData(dataName: str, processLine: Callable[[Dict[str, any], Callable[[str], None]], None]):
    with open(dataFile(f"yelp_{dataName}.JSON"), 'r') as f:
        outfile = open(outputFile(f"yelp_{dataName}_output.txt"), 'w')
        count_line = 0
        # read each JSON abject and extract data
        for line in f:
            data = json.loads(line)
            outfile.write(f"{count_line} - ")
            processLine(data, outfile.write)
            count_line += 1
    print(count_line)
    outfile.close()
    f.close()


def writeBusinessData(data: Dict[str, any], write: Callable[[str], None]):
    write("business info : '{}' ; '{}' ; '{}' ; '{}' ; '{}' ; '{}' ; {} ; {} ; {} ; {}\n".format(
        cleanStr4SQL(data['business_id']),
        cleanStr4SQL(data["name"]),
        cleanStr4SQL(data["address"]),
        cleanStr4SQL(data["state"]),
        cleanStr4SQL(data["city"]),
        cleanStr4SQL(data["postal_code"]),
        str(data["latitude"]),
        str(data["longitude"]),
        str(data["stars"]),
        str(data["is_open"])))
    
    # process business categories
    categories = data["categories"].split(', ')
    write("      categories: {}\n".format(str(categories)))

    # TO-DO : write your own code to process attributes
    # make sure to **recursively** parse all attributes at all nesting levels. You should not assume a particular nesting level.
    write("      attributes: {}\n".format(
        str(flatten(data["attributes"]))))

    # TO-DO : write your own code to process hours data
    write("      hours: {}\n".format(str(
        list(map(lambda item: (item[0], item[1].split('-')), data["hours"].items())))))


def flattenHelper(flattenedItems: List[Tuple[str, any]], item: Tuple[str, any]) -> List[Tuple[str, any]]:
    if(isinstance(item[1], dict)):
        # Does not concatenate names
        # flattenedItems.extend(flatten(item[1]))

        # Does concatenate names
        flattenedItems.extend(map(lambda nestedItem: (
            f"{item[0]}.{nestedItem[0]}", nestedItem[1]), flatten(item[1])))
    else:
        flattenedItems.append(item)
    return flattenedItems


def flatten(attribute: Dict[str, any]) -> List[Tuple[str, any]]:
    return reduce(flattenHelper, attribute.items(), [])


def writeUserData(data: Dict[str, any], write: Callable[[str], None]):
    write("user info : '{}' ; '{}' ; '{}' ; {} ; {} ; {} ; ({},{},{})\n".format(
        cleanStr4SQL(data['user_id']),
        cleanStr4SQL(data["name"]),
        cleanStr4SQL(data["yelping_since"]),
        str(data["tipcount"]),
        str(data["fans"]),
        str(data["average_stars"]),
        str(data["funny"]),
        str(data["useful"]),
        str(data["cool"])))
    write("      friends: {}\n".format(str(data["friends"])))


def writeCheckinData(data: Dict[str, any], write: Callable[[str], None]):
    write(f"'{cleanStr4SQL(data['business_id'])}':\n")
    dates: List[str] = data["date"].split(",")
    write(" 	".join(map(lambda datetime: str(tuple(
        re.split(r'[- ]', datetime))), dates)))
    write("\n")


def writeTipData(data: Dict[str, any], write: Callable[[str], None]):
    write(f"'{cleanStr4SQL(data['business_id'])}' ; '{cleanStr4SQL(data['date'])}' ; {str(data['likes'])} ; '{cleanStr4SQL(data['text'])}'; '{cleanStr4SQL(data['user_id'])}'\n")


if __name__ == "__main__":
    parseData("business", writeBusinessData)
    parseData("user", writeUserData)
    parseData("checkin", writeCheckinData)
    parseData("tip", writeTipData)
