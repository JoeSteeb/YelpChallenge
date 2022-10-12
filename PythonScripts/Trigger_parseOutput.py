import os
import argparse
import json
import psycopg2
import re
args = {}

#assuming: linenum - business info : business_id ; name ; address ; state ; city ; zip ; longitude ; latitude ; stars ; is-open
                    # categories : [x, y, z]
                    # attributes : [(x, t/f), (y, t/f)]
                    # hours : [(day, (open, close)]
def insertBusinessTable(filename):
    with open(filename, 'r') as file:
        eof = False
        count = 1
        while not eof:
            business_info = file.readline().split(': ')[1].split(' ; ')
            categories = file.readline().replace(' ', '').split(':')[1]
            attributes = file.readline().replace(' ', '').split(':')[1]
            hours = file.readline().replace(' ', '').replace('hours:', '')
            print("business_info\n", business_info)
            print("categories\n", categories)
            print("attributes\n", attributes)
            print("hours\n", hours)
            # sql_str = "INSERT INTO Business(business_id, name, address, state, city, zip, longitude, latitude, stars, is_open, tip_count, numCheckins)" + " VALUES ({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})"
            # insertBusinessInfo(sql_str, business_info)
            # insertCategoriesInfo(sql_str, categories)
            sql_str = "INSERT INTO Hours(business_id, dayofweek, open, close)" + " VALUES ({1}, {2}, {3}, {4})"
            insertHoursInfo("sql_str", business_info[0], hours)
            if count == 1:
                break


# def insertBusinessInfo(sql_str, business_info):
def insertHoursInfo(sql_str, business_id, hours):
    try:
        conn = psycopg2.connect("dbname='yelpdb' user='postgres' host='localhost' password='pswd'")
    except:
        print("unable to connect to yelpdb")

    cur = conn.cursor()
    hours = hours.split(',')
    print("hours:\n", hours)
    len_hours = len(hours)
    counter = 0
    sql_str.replace('{1}', business_id).replace('{2}', hours[0].strip('[(')).replace('{3}', hours[1].strip('[')).replace('{4}', hours[2].strip('])'))
    # while len_hours > 0:

        # try:
        #     cur.execute(sql_str.replace('{1}', business_id).replace('{2}', hours[0].strip('[(')).replace('{3}', hours[1].strip('[')).replace('{4}', hours[2].strip('])')))

    cur.close()
    conn.close()



def main_function():
    # assuming yelp_table_output.txt
    t1 = gmtime()
    outputFileList = {}
    for file in os.listdir(args.output):
        outputFileList[file.replace('yelp_', '').replace('_output.txt', '')] = os.path.join(args.output, file)

    insertBusinessTable(outputFileList['business'])
    print("insert time", gmtime() - t1)
    # insertUserTable(outputFileList['user'])
    # insertTipTable(outputFileList['tip'])
    # insertCheckinTable(outputFileList['checkin'])


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description="Parses through database outputs to generate SQL INSERT statements. Precondition: Tables must be created and ouput data must match schema.")
    parser.add_argument('-o', '--output', metavar='OUTPUT_PATH', required=True, help="required path to output directory (which we intend to read from)")
    parser.add_argument('-d', '--database', metavar='DATABASE', default='hw4', help='SQL database to INSERT into')
    parser.add_argument('-u', '--username', metavar='USERNAME', default='postgres', help='Username for database')
    parser.add_argument('-p', '--password', metavar='PASSWORD', required=True, help='Password for database')
    args = parser.parse_args()

    main_function()