
from flask import Flask, send_from_directory, render_template, request, redirect
from flask_cors import CORS
from dotenv import load_dotenv
import os
import json
import uuid
import DataCollector
import shutil
from werkzeug.exceptions import InternalServerError, abort


load_dotenv()

join = os.path.join
HASH_PATH = os.getenv("VENDOR_HASH_PATH")
JSON_PATH = os.getenv("VENDOR_JSON_PATH")
SOURCE_PATH = os.getenv("VENDOR_SOURCE_PATH")

app = Flask(__name__)
CORS(app)

def make_tree(path):
    tree = dict(name=os.path.basename(path), children=[])
    try: lst = os.listdir(path)
    except OSError:
        pass #ignore errors
    else:
        for name in lst:
            fn = os.path.join(path, name)
            if os.path.isdir(fn):
                tree['children'].append(make_tree(fn))
            else:
                tree['children'].append(dict(name=name))
    return tree

def make_list(path):
    _list = []
    try: lst = os.listdir(path)
    except OSError:
        pass #ignore errors
    else:
        for name in lst:
            fn = os.path.join(path, name)
            if os.path.isdir(fn):
                _list.append(create_link( os.path.join(fn, "info.json"), name))

    print(fn)
    return _list

def read_info(path, folderName):

    if folderName is None:
        return [{'Name', 'hide'}]

    with open(os.path.join(path,folderName,"info.json")) as f:
        data = json.load(f)
    return data

def prepare_info(json, folderName):

    json['Folder'] = folderName
#    json['Links'] = {}
#
#    if json['LinkWebsite'] is not None:
#        json['Links']['Website'] = json['LinkWebsite']
#
#    if json['LinkFacebook'] is not None:
#        json['Links']['Facebook'] = json['LinkFacebook']
#
#    if json['LinkInstagram'] is not None:
#        json['Links']['Instagram'] = json['LinkInstagram']
#
#    if json['LinkZoom'] is not None:
#        json['Links']['Zoom'] = json['LinkZoom']

    return json;

def create_link(path, folderName):
    with open(path) as f:
        data = json.load(f)
        name = data['Name'];

    return dict(Folder=folderName, Name=name)

def process_list_edit_actions(_dict):
    if 'NewVendor' in _dict.keys():

        print("create new...")

        dirName = os.path.join(SOURCE_PATH,str(uuid.uuid4()))
        os.mkdir(dirName)

        with open(os.path.join(SOURCE_PATH,"default.json")) as f:
            defaultJson = json.load(f)

        with open(dirName + "/info.json", "w+") as new:
            json.dump(defaultJson, new)

        return True

    elif 'Rebuild' in _dict.keys():

        print("rebuilding...")
        DataCollector.generate_data_json(True)

        return True

    elif 'Clear' in _dict.keys():

        for toDelete in _dict.values():
            print("delete: ", toDelete)
            path = os.path.join(SOURCE_PATH,toDelete)
            shutil.rmtree(path)

        return True

    return False

@app.route('/details', methods=["GET", "POST"])
def details():

    folder = None

    print(request.args)

    if request.args:
        folder = request.args.get("Folder")

    jsonFile = read_info(SOURCE_PATH, folder)

    if request.method == "POST":

        print("files: ", request.files)
        print("forms: ", request.form)

        if request.files:

            values_view =request.files.values()
            value_iterator = iter(values_view)
            image = next(value_iterator)

            values_view =request.files.keys()
            value_iterator = iter(values_view)
            toSplit = next(value_iterator)

            folderName, prefix = toSplit.split('/')

            #here help ajudame!

            name = prefix + "_" + str(uuid.uuid4())

            path = join(SOURCE_PATH, folderName, name)
            image.save(path)

            print("save image to: " + path)

            return redirect(request.url)

        elif request.form:

            _dict = request.form

            if process_list_edit_actions(_dict):

                print("edited list...")

            elif 'ChangeLink' in _dict.keys():

                for string in _dict.values():

                    pair = string.split(',')
                    key = pair[0]
                    value = pair[1]

                    print("CHANGED LINK", key, " FROM ", jsonFile["Links"][key], " TO ", value)
                    jsonFile["Links"][key] = value

            else:
                for key in _dict:

                    print("CHANGED ", key, " => \n ", jsonFile[key], "\n TO => \n ", _dict[key])
                    jsonFile[key] = _dict[key]

                path = os.path.join(SOURCE_PATH,folder,"info.json")
                with open(path,'w') as outfile:
                    json.dump(jsonFile, outfile)
                    print("saved changes to: ", path)

    info = prepare_info(jsonFile, folder)
    #print("")
    #print(info)

    return render_template('details.html', list=make_list(SOURCE_PATH), detail=info)
        

@app.route('/', methods=["GET", "POST"])
def index():

    folder = None

    print(request.args)

    if request.method == "POST":

        print("forms: ", request.form)

        if request.form:

            _dict = request.form

            process_list_edit_actions(_dict);
            
    return render_template('index.html', list=make_list(SOURCE_PATH))


@app.route('/tree')
def dirtree():
    path = os.path.expanduser(u'~')
    return render_template('dirtree.html', tree=make_tree(SOURCE_PATH))


@app.route("/vendor_hash",methods = ['GET'])
def get_vendor_hash():
    try:
        with open(HASH_PATH,"r") as file:
            hash = file.readline()
            return hash

    except Exception as e:
        raise InternalServerError(str(type(e)) + ": " + str(e))

@app.route("/vendor_data",methods = ['GET'])
def get_vendor_data():
    #hardcoded Data.json has length 9! Should split path
    return send_from_directory(JSON_PATH[:-9],"data.json")

@app.route("/get/<path:subpath>_<string:filename>")
def get_file(subpath, filename):
    path = join(SOURCE_PATH,subpath)

    if(os.path.isfile(join(path,filename))):
        return send_from_directory(path, filename)
    else:
        return abort(404)

if __name__=='__main__':
    app.run(host="0.0.0.0", port=8082, threaded = True, debug=True, ssl_context =('/root/ssl/cert.crt', '/root/ssl/private.key'))