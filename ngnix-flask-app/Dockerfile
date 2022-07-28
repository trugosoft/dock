# pull the image from docker hub
FROM continuumio/miniconda3:latest

# adds metadata to an image
LABEL MAINTAINER="Trugo"
LABEL GitHub="https://github.com/bpostance/deng.learn/tree/master/docker"
LABEL version="0.0"
LABEL description="A Docker container to serve a simple Python Flask API"

## Override the default shell (not supported on older docker, prepend /bin/bash -c )
SHELL ["/bin/bash", "-c"]

# Set WORKDIR - the working directory for any RUN, CMD, ENTRYPOINT, COPY and ADD instructions that follow it in the Dockerfile
WORKDIR /home/flask-app

#install dependencies
RUN pip install --upgrade pip
COPY requirements.txt ./
RUN pip install -r requirements.txt

RUN apt-get update \
    && apt-get install -y nginx gunicorn supervisor 

# COPY - copies files or directories from <src> and adds them to the filesystem of the container at the path <dest>.
COPY environment.yml ./

# ADD - "adds" directories and their contents to the container
ADD app ./app

# Setup nginx
RUN rm /etc/nginx/sites-enabled/default
COPY flask.conf /etc/nginx/sites-available/
RUN ln -s /etc/nginx/sites-available/flask.conf /etc/nginx/sites-enabled/flask.conf
RUN echo "daemon off;" >> /etc/nginx/nginx.conf


# conda set-config and create environment based on .yml
# chain seperate multi-line commands using '&& \'
RUN conda env update -f environment.yml

# set env variables
RUN echo ". /opt/conda/etc/profile.d/conda.sh" >> ~/.bashrc && \
    echo "conda activate" >> ~/.bashrc

# Setup supervisord
RUN mkdir -p /var/log/supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
COPY gunicorn.conf /etc/supervisor/conf.d/gunicorn.conf

# Start processes
CMD ["/usr/bin/supervisord"]