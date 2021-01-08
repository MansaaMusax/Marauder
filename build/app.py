import uvicorn as uvicorn
from fastapi import FastAPI, Depends, HTTPException
from starlette.status import HTTP_500_INTERNAL_SERVER_ERROR, HTTP_400_BAD_REQUEST
from factionpy.fastapi import user_has_write_access
from factionpy.logger import log
from factionpy.models import HasuraRequest, User

from build.payloads import get_payload_name
from build.payloads.http import build_http_payload
from build.responses import BuildResponse

app = FastAPI()


@app.post('/', response_model=BuildResponse)
async def build(hr: HasuraRequest, user: User = Depends(user_has_write_access)):
    try:
        payload_name = get_payload_name(str(hr.new_data['payload_type_id']))
        if payload_name == "MarauderHTTP":
            return build_http_payload(hr.new_data)
        else:
            log(f"Unknown payload name: {payload_name}", "error")
            raise HTTPException(
                status_code=HTTP_400_BAD_REQUEST, detail=f"Unknown payload name: {payload_name}"
            )
    except Exception as e:
        raise HTTPException(
            status_code=HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Error with request: {e}"
        )

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
