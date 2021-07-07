
from flask import Flask, send_from_directory, render_template, request, redirect
from flask_cors import CORS
from dotenv import load_dotenv
import os
import json
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
            fn = os.path.join(path, name, "info.json")
            _list.append(create_link(fn, name))

    return _list

def read_info(path, folderName):

    if folderName is None:
        return [{'Name', 'hide'}]

    with open(os.path.join(path,folderName,"info.json")) as f:
        data = json.load(f)
        data['Folder'] = folderName
        data['Links'] = {}

        if data['LinkWebsite'] is not None:
            data['Links']['Website'] = data['LinkWebsite']

        if data['LinkFacebook'] is not None:
            data['Links']['Facebook'] = data['LinkFacebook']

        if data['LinkInstagram'] is not None:
            data['Links']['Instagram'] = data['LinkInstagram']

        if data['LinkZoom'] is not None:
            data['Links']['Zoom'] = data['LinkZoom']

    return data

def create_link(path, folderName):
    with open(path) as f:
        data = json.load(f)
        name = data['Name'];

    return dict(Folder=folderName, Name=name)

@app.route('/details', methods=["GET", "POST"])
def details():

    print(request.method)

    folder = None

    print(request.args)

    if request.args:
        folder = request.args.get("Folder")

    elif request.method == "POST":

        print(request.files)

        if request.files:

            values_view =request.files.values()
            value_iterator = iter(values_view)
            image = next(value_iterator)

            values_view =request.files.keys()
            value_iterator = iter(values_view)
            name = next(value_iterator)

            path = join(SOURCE_PATH, name)

            
            image.save(path)

            print("save image to: " + path)

            return redirect(request.url)

    info = read_info(SOURCE_PATH, folder)
    print(info)

    return render_template('details.html', list=make_list(SOURCE_PATH), detail=info)
        

@app.route('/', methods=["GET", "POST"])
def index():

    if request.method == "POST":

        print(request.files)

        if request.files:

            values_view =request.files.values()
            value_iterator = iter(values_view)
            image = next(value_iterator)

            values_view =request.files.keys()
            value_iterator = iter(values_view)
            name = next(value_iterator)

            path = join(SOURCE_PATH, name)

            
            image.save(path)

            print("save image to: " + path)

            return redirect(request.url)

    folder = None
    if request.args:
        folder = request.args.get("Folder")

    info = None
    if folder:
        info = read_info(SOURCE_PATH, folder)

    print(info)
    return render_template('index.html', list=make_list(SOURCE_PATH), detail=info)


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