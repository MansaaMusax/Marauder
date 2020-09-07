import os
from flask import Flask
from flask_restx import Api, Resource, reqparse
from factionpy.logger import log
from factionpy.flask import FactionApp
from factionpy.flask.auth import authorized_groups, current_user

app = Flask(__name__)
app.config["LOG_TYPE"] = os.environ.get("LOG_TYPE", "stream")
app.config["LOG_LEVEL"] = os.environ.get("LOG_LEVEL", "DEBUG")
api = Api(app, version='1.0', title='Faction File Service API',
          description='A service for building Marauder payloads',
          )
faction_app = FactionApp(app_name="marauder-build", app=app)

parser = reqparse.RequestParser()
parser.add_argument('agent_config', required=True, type=str, help="Agent configuration as JSON string"),
parser.add_argument('transport_config', required=True, type=str, help="Transport configuration as JSON string"),

def build_payload(agent_config: dict, transport_config: dict):


@api.route('/')
class Build(Resource):
    @authorized_groups(["standard_write"])
    def post(self):
        args = parser.parse_args()
