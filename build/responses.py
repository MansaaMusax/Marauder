from uuid import UUID

from pydantic import BaseModel


class BuildResponse(BaseModel):
    id: UUID
