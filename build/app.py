import os
import json
from subprocess import call

from flask import Flask, request
from flask_restx import Api, Resource, reqparse
from factionpy.logger import log, error_out
from factionpy.flask import FactionApp
from factionpy.flask.auth import authorized_groups, current_user
from factionpy.services import HasuraRequest

from build.payloads import get_payload_name
from build.payloads.http import build_http_payload

app = Flask(__name__)
app.config["LOG_TYPE"] = os.environ.get("LOG_TYPE", "stream")
app.config["LOG_LEVEL"] = os.environ.get("LOG_LEVEL", "DEBUG")
api = Api(app, version='1.0', title='Marauder Build Service',
          description='A build for building Marauder payloads',
          )
faction_app = FactionApp(app_name="marauder-build", app=app)


@api.route('/')
class Build(Resource):
    @authorized_groups(["standard_write"])
    def post(self):
        try:
            hr = HasuraRequest(request.json)
            payload_name = get_payload_name(hr.new_data['payload_type_id'])
            if payload_name == "Marauder HTTP":
                build_http_payload(request.json)
            else:
                log(f"Unknown payload name: {payload_name}", "error")
        except Exception as e:
            return {"success": "false", "result": f"Error with request: {e}"}
