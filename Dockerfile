# Dockerfile for a .NET MAUI project — builds the Android target and runs unit tests.
# NOTE: This can only build/test Android. iOS cannot be built here (requires macOS + Xcode).
#
# ---- Things to adjust for your project ----
#   1. TEST_PROJECT_PATH below -> path to your test .csproj
#   2. android-34 / build-tools;34.0.0 -> match your MAUI project's target Android API level
#   3. net8.0-android -> match your MAUI project's actual TFM if different (e.g. net9.0-android)

FROM mcr.microsoft.com/dotnet/sdk:10.0

# ---- Java (required by Android SDK tooling) ----
RUN apt-get update && apt-get install -y --no-install-recommends \
    openjdk-17-jdk \
    wget \
    unzip \
    && rm -rf /var/lib/apt/lists/*

ENV JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64

# ---- Android SDK command-line tools ----
ENV ANDROID_SDK_ROOT=/opt/android-sdk
RUN mkdir -p ${ANDROID_SDK_ROOT}/cmdline-tools && \
    wget -q https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip -O /tmp/cmdline-tools.zip && \
    unzip -q /tmp/cmdline-tools.zip -d ${ANDROID_SDK_ROOT}/cmdline-tools && \
    mv ${ANDROID_SDK_ROOT}/cmdline-tools/cmdline-tools ${ANDROID_SDK_ROOT}/cmdline-tools/latest && \
    rm /tmp/cmdline-tools.zip

ENV PATH=${PATH}:${ANDROID_SDK_ROOT}/cmdline-tools/latest/bin:${ANDROID_SDK_ROOT}/platform-tools

# Accept SDK licenses and install the packages MAUI needs to build for Android
RUN yes | sdkmanager --licenses > /dev/null 2>&1; \
    sdkmanager "platform-tools" "platforms;android-36" "build-tools;36.0.0"

# ---- .NET MAUI workload (Android component only — iOS workload is useless on Linux) ----
RUN dotnet workload install maui-android --ignore-failed-sources

WORKDIR /src

# Copy the whole solution in (see .dockerignore to keep this fast/clean)
COPY . .

# Path to your main MAUI App project — CHANGE THIS to match your solution.
# You do NOT need to list your class library project separately: dotnet build
# automatically builds any <ProjectReference> the App project depends on,
# including class libraries. Targeting the App .csproj directly (instead of
# the whole .sln) also avoids trying to build an iOS head, which would fail
# here since this is a Linux container with no Xcode/iOS workload.
ENV APP_PROJECT_PATH=InventoryManagement/InventoryManagement.csproj

RUN dotnet restore ${APP_PROJECT_PATH}

# Build the Android target only (this pulls in the class library automatically).
# NOTE: built to an internal path, NOT /src/build-output directly — that path
# gets mounted over by a host volume at "docker run" time, which would hide
# anything placed there during the build. The CMD below copies the APK out
# to the mounted folder at container startup instead.
RUN dotnet build ${APP_PROJECT_PATH} -f net10.0-android -c Release -o /opt/app-build-output

# When the container runs: copy the built APK(s) into the mounted host folder.
CMD ["sh", "-c", "mkdir -p /src/build-output && cp -v /opt/app-build-output/*.apk /src/build-output/ && echo 'APK copied to mounted build-output folder.'"]
