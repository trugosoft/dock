FROM golang:1.16-alpine AS base
WORKDIR /app

ENV GO111MODULE="on"
ENV GOOS="linux"
ENV CGO_ENABLED=0

RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64

RUN apk update \
    && apk add --no-cache \
    ca-certificates \
    curl \
    tzdata \
    git \
    && update-ca-certificates

FROM base AS dev
WORKDIR /app

RUN go get -u github.com/cosmtrek/air && go install github.com/go-delve/delve/cmd/dlv@latest
EXPOSE 5000
EXPOSE 2345

ENTRYPOINT ["air"]

FROM base AS builder
WORKDIR /app

COPY . /app
RUN go mod download \
    && go mod verify 

RUN go build -o todo -a .


FROM alpine:latest as prod
COPY --from=builder /app/todo /usr/local/bin/todo
COPY .env /usr/local/bin/todo/.env
RUN chmod -R +x /usr/local/bin/todo

#RUN ls -al /usr/local/bin/todo
#RUN cat /usr/local/bin/todo/.env
EXPOSE 5000

ENTRYPOINT ["/usr/local/bin/todo/go-todo"]