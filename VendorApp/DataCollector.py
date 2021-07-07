from genericpath import isdir
from ntpath import join
import zipfile
from dotenv import load_dotenv
import os
import json
import hashlib
from zipfile import ZipFile

load_dotenv()

join = os.path.join

SOURCE_PATH =os.getenv("VENDOR_SOURCE_PATH")
HASH_PATH = os.getenv("VENDOR_HASH_PATH")
JSON_PATH = os.getenv("VENDOR_JSON_PATH")

def generate_data_json(showZoomLinks):
    print ("Collecting Data")
    print ("Path: " + SOURCE_PATH)

    #get all vendor folders
    directories = [name for name in os.listdir(SOURCE_PATH) if os.path.isdir(join(SOURCE_PATH, name))]

    vendors = []

    for directory_name in directories:
        data_path = join(SOURCE_PATH, directory_name, "info.json")

        try:
            with open(data_path, 'r') as file:
                json_obj = json.load(file)

                print("Found " + json_obj["Name"])

                json_obj["Directory"] = directory_name

                vendors.append(json_obj)
        
        except (FileNotFoundError ,IOError) as e:
            print ("Error reading file: " , e)

    #calculate hash and compare to previous
    vendorsString = json.dumps(vendors) + str(showZoomLinks);
    hash = hashlib.md5(vendorsString.encode("utf-8")).hexdigest()
    print("Vendors Hash is: " + hash + " , comparing to " + HASH_PATH)
    same_hash = False

    try:
        with open(HASH_PATH,"r") as file:
            old_hash = file.readline()
            print("Old Hash: " + old_hash)
            if old_hash == hash:
                same_hash = True
        
    except FileNotFoundError:
        print("No Hash file found.")

    if same_hash:
        print ("Hash matches cached version. Terminating.")
        return
    else:
        with open(HASH_PATH,"w+") as file:
            print("Hash differes, saving new hash")
            file.seek(0)
            file.truncate()
            file.write(hash)


    #compose final JSON
    print("Generating JSON")
    finalJSON = {}
    finalJSON["Hash"] = hash
    finalJSON["Vendors"] = vendors
    finalJSON["ShowZoomLinks"] = showZoomLinks

    with open(JSON_PATH, "w+") as file:
        json.dump(finalJSON, file, indent=2)

    return


if __name__ == "__main__":
    generate_data_json(showZoomLinks = True)