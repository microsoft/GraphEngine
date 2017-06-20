FROM ubuntu
WORKDIR /app
RUN apt update
RUN apt install -y --no-install-recommends \
        curl \
        libssl-dev \
        ca-certificates \
        libc6 \
        libcurl3 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu55 \
        liblttng-ust0 \
        libssl1.0.0 \
        libstdc++6 \
        libunwind8 \
        libuuid1 \
        zlib1g
RUN mkdir /coreclr && \
    cd /coreclr && \
    curl --tlsv1 -SL https://dotnetcli.blob.core.windows.net/dotnet/Sdk/master/dotnet-dev-ubuntu.16.04-x64.latest.tar.gz | tar xz && \
    ln -s /coreclr/dotnet /usr/bin/ && \
    mkdir warmup && \
    cd warmup && \
    dotnet new && \
    cd .. && rm -rf warmup

# In your own Dockerfile, copy your app in, and specify an entry point:
# COPY out .
# ENTRYPOINT dotnet your_app.dll