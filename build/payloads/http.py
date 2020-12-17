import json
from subprocess import call
from flask import current_app

from factionpy.logger import log, error_out


def build_http_payload(payload_config: dict):
    log(f"Received payload config: {payload_config}", "debug")
    staging_id = payload_config["id"]
    staging_key = payload_config["staging_key"]

    log(f"Received build config: {payload_config['configuration']}")

    try:
        build_config = json.loads(payload_config["configuration"])
    except Exception as e:
        return error_out(f"Could not load configuration: {e}")

    marauder_settings = dict({
        "StagingId": staging_id,
        "StagingKey": staging_key,
        "BeaconInterval": build_config["beacon_interval"],
        "Jitter": build_config["jitter"],
        "ExpirationDate": build_config["expiration_date"],
        "Debug": build_config["debug"],
        "URLs": build_config["urls"],
        "Headers": build_config["headers"],
        "Cookies": build_config["cookies"],
        "MessageConfig": build_config["message_config"],
        "ServerResponseConfig": build_config["server_response_config"]
    })

    log("[Marauder Build] Writing agent config values to ./settings.json")
    with open('./src/MarauderHTTP/settings.json', 'w') as settings_file:
        json.dump(marauder_settings, settings_file)

    if build_config["debug"]:
        configuration = "Debug"
        output_path = "./src/MarauderHTTP/bin/Debug/Marauder.exe"
    else:
        configuration = "Release"
        output_path = "./src/MarauderHTTP/bin/Release/Marauder.exe"

    restore_cmd = "nuget restore"
    log(f"[Marauder Build] Running restore command: {restore_cmd}", "debug")
    restore_exit = call(restore_cmd, shell=True, cwd="./src/")

    if restore_exit == 0:
        # Setup version
        if build_config["version"] == "NET35":
            version = "v3.5"
        elif build_config["version"] == "NET45":
            version = "v4.5.2"
        else:
            return error_out("[Marauder Build] Could not find a match for version: {}".format(build_config["Version"]))

        # Setup Debug vs Release and build
        build_cmd = f"msbuild MarauderHTTP.csproj /t:Build /p:Configuration={configuration} /p:TargetFrameworkVersion={version} "
        log(f"[Marauder Build] Running service command: {build_cmd}")
        build_exit = call(build_cmd, shell=True, cwd="./src/MarauderHTTP/")
    else:
        return error_out("[Marauder Build] Failed to restore packages.")

    if build_exit == 0:
        result = current_app.faction.client.upload_file("payload", output_path, description=f"Marauder Payload: {staging_id}")
        if result['success']:
            return dict({
                "success": True,
                "message": ""
            })
        else:
            return error_out(f"Failed to upload payload. Response: {result['message']}")
    else:
        return error_out("[Marauder Build] Build Failed.")

