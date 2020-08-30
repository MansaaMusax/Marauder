FROM mono:6
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN apt-get update && \
  apt-get install python3 -y

# copy csproj and restore as distinct layers


# copy and build everything else
COPY . ./
RUN nuget restore
ENTRYPOINT [ "python3", "./build/app.py" ]