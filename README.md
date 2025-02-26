# Elastic Kibana Example

## Getting Started 

1. Change the two passwords in the _.env_ file 
2. Use terminal to execute `docker compose up`
3. Copy the certicate in the setup service to your local drive:
   - `docker cp elastic-kibana-example-es01-1:/usr/share/elasticsearch/config/certs/ca/ca.crt /temp/`
4. Now it should be possible to access the Elastic server via Curl (the result is a JSON with server info)
   - `curl --cacert /temp/ca.crt -u elastic:changeme https://localhost:9200 -k`
5. The components should also be accessible using the browser, user "elastic" and password "changeme"
    - Elastic Search on [localhost:9200](https://localhost:9200/) 
    - Kibana on [localhost:5601](http://localhost:5601/) 

## Additional Links

- [Getting started with the Elastic Stack and Docker Compose: Part 1](https://www.elastic.co/blog/getting-started-with-the-elastic-stack-and-docker-compose)