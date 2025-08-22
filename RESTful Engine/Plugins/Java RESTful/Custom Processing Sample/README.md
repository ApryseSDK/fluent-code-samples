
# Fluent Java RESTful Custom Processing Sample

This sample demonstrates how to implement a custom post-processor for the Java RESTful Engine. The processor removes the data from the document, so when you get the document after processing, the "Data" will be an empty byte array.

## Prerequisites
- Java 8 or higher
- The Java RESTful Engine (Tomcat or Docker installation)
- Maven (for building the sample)

## Usage

### 1. Clone or Download this Repository

### 2. Build the Custom Processor

1. Navigate to the `CustomProcessingSampleJava` directory.
2. Add the Java RESTful Engine SDK JARs to your classpath (or as Maven dependencies if available).
3. Build the project using Maven:
   ```sh
   mvn clean package
   ```
   This will create a JAR file in the `target` directory.

### 3. Deploy the Custom Processor

#### Tomcat Installation

1. Copy the compiled `CustomProcessor.class` (or JAR) to the `lib` or `classes` directory of your RESTful Engine Tomcat installation.
2. Edit the `application.properties` (or equivalent config) for the RESTful Engine and add:
   ```properties
   postProcessor=com.example.customprocessing.CustomProcessor
   ```
3. Restart Tomcat.

#### Docker Installation

1. Build your custom processor into a JAR file.
2. Create a Dockerfile that extends the official Java RESTful Engine image and copies your JAR into the appropriate directory.
   Example Dockerfile:
   ```dockerfile
   FROM apryse/fluent-restful-engine-java:latest
   COPY target/CustomProcessingSampleJava.jar /opt/restfulengine/lib/
   ENV POST_PROCESSOR=com.example.customprocessing.CustomProcessor
   ```
3. Build and run your Docker image:
   ```sh
   docker build -t custom-restful-engine .
   docker run -p 8080:8080 custom-restful-engine
   ```

### 4. Test the Processor

Start your RESTful Engine and run a report. The returned document's "Data" will be empty.

## Documentation
See the [official documentation](https://fluent.apryse.com/documentation/engine-guide/Fluent%20RESTful%20Engines/JavaRESTCustomProc) for more details.
