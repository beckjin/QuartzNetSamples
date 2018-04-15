docker build -t beckjin/test-quartznet-local .
docker tag beckjin/test-quartznet-local:latest beckjin/test-quartznet:0.0.1
docker push beckjin/test-quartznet:0.0.1
docker pull beckjin/test-quartznet:0.0.1