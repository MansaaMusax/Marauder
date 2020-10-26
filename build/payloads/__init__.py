from gql import gql
from flask import current_app

from factionpy.logger import log


def get_payload_name(payload_id: str):
    query = """
query Payload {
  payload_types(where: {{id: {_eq: "PAYLOAD_ID"}}) {
    name
  }
}
""".replace("PAYLOAD_ID", payload_id)
    result = None

    try:
        result = current_app.faction_app.client.execute(gql(query))
    except Exception as e:
        log(f"Could not get Marauder ID. Error {e}", "error")
        return None

    if result:
        try:
            if len(result['payload_types']) > 0:
                return result['payload_types'][0]['name']
        except Exception as e:
            log(f"Could not get Payload ID from result. Error {e}", "error")
            return None
    else:
        log(f"No result returned from Hasura. That's weird.", "info")
    return None