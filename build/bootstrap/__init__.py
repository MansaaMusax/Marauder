from gql import gql
from flask import current_app

from factionpy.logger import log, error_out


GET_MARAUDER_ID = """
query MarauderId {
  agent_types(where: {name: {_eq: "Marauder"}}) {
    id
  }
}
"""

CREATE_MARAUDER = """
mutation CreateMarauder {
  insert_agent_types(objects: {
    development: false, 
    authors: "{@jaredhaight}", 
    module_types: "{dotnet}", 
    name: "Marauder"}) {
    returning {
      id
    }
  }
}
"""

# ec6de1e7-04b5-4c4c-b0e8-b73b61796c88
CREATE_HTTP_AGENT_PAYLOAD_TYPE = """
mutation CreateHTTPAgentPayload($marauder_id:guid, $default_config:json) {
  insert_payload_type_one(object: {
    name: "Marauder HTTP", 
    agent_type_id: $marauder_id, 
    description: "HTTP agent for Marauder",
    transport_guid: "2daece20-0d27-4068-b265-ceff27d3f3b2",
    configuration_schema: $default_config
  })
}
"""

HTTP_PAYLOAD_VARIABLES = """
{
  "marauder_id": "MARAUDER_ID",
  "default_config": {
    "beacon_internal": 5,
    "jitter": 25,
    "expiration_date": "",
    "operating_system": "windows",
    "version": "NET35",
    "architecture": "x86",
    "debug": true,
    "urls": [
      "foo1",
      "foo2"
    ],
    "headers": [
      {
        "name": "foo",
        "value": "vfoo"
      }
    ],
    "cookies": [
      {
        "name": "foo",
        "value": "vfoo"
      }
    ],
    "message_config": {
      "defaut": "foo",
      "prepend": "bar",
      "append": "baz"
    },
    "server_response_config": {
      "defaut": "foo",
      "prepend": "bar",
      "append": "baz"
    }
  }
}
"""


def get_marauder_id():
    result = None

    try:
        result = current_app.faction.graphql(gql(GET_MARAUDER_ID))
    except Exception as e:
        log(f"Could not get Marauder ID. Error {e}", "error")
        return None

    if result:
        try:
            if len(result['agent_types']) > 0:
                return result['agent_types'][0]['id']
        except Exception as e:
            log(f"Could not get Marauder ID from result. Error {e}", "error")
            return None
    else:
        log(f"No result returned from Hasura. That's weird.", "info")
    return None


def register_marauder():
    marauder_id = get_marauder_id()
    if not marauder_id:
        try:
            current_app.faction.graphql(gql(CREATE_MARAUDER))
        except Exception as e:
            log(f"Could not create Marauder. Error: {e}", "error")


def register_http_payload():
    marauder_id = get_marauder_id()
    if marauder_id:
        try:
            variables = HTTP_PAYLOAD_VARIABLES.replace("MARAUDER_ID", marauder_id)
            current_app.faction.graphql(gql(CREATE_HTTP_AGENT_PAYLOAD_TYPE), variable_values=variables)
        except Exception as e:
            log(f"Could not create Marauder HTTP Payload. Error: {e}", "error")
    else:
        log(f"Could not create Marauder HTTP payload. No Marauder ID found.", "error")
