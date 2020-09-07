FROM mono:6
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

COPY . ./

RUN apt-get update && \
  apt-get install python3 pip3 -y && \
  pip3 installp pipenv && \
  pipenv lock --requirements > ./requirements.txt && \
  pip3 install -r requirements.txt && \
  nuget restore

ENTRYPOINT [ "python3", "./build/app.py" ]