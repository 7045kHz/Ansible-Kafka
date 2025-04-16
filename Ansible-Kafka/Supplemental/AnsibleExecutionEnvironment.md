```sh
$ ansible-builder build -t kafka-ee -f execution-environment.yml
$ podman push --tls-verify=false kafka-ee artifactory.example.com:5000/kafka-ee

# Edit AAP to add repo kafka-ee
# Edit Job Template to use kafka-ee

# cat execution-environment.yml
```
```yaml execution-environment.yml
---
version: 3.1

build_arg_defaults:
  ANSIBLE_GALAXY_CLI_COLLECTION_OPTS: '--pre'

dependencies:
  ansible_core:
    package_pip: ansible-core==2.15.8
  ansible_runner:
    package_pip: ansible-runner
  python:
    - six
    - psutil
    - confluent_kafka
    - dumb-init>=1.2.5
  system:
    - iputils
    - openssh
    - libssh-config
    - libssh
    - openssh-clients
    - sshpass

images:
  base_image:
    name: docker.io/redhat/ubi9:latest

    #  docker.io/redhat/ubi9:latest
    # Other available base images:
    #   - quay.io/rockylinux/rockylinux:9
    #   - quay.io/centos/centos:stream9
    #   - registry.fedoraproject.org/fedora:38
    #   - registry.redhat.io/ansible-automation-platform-23/ee-minimal-rhel8:latest
    #     (needs an account)

# Custom package manager path for the RHEL based images
options:
  package_manager_path: /usr/bin/dnf
  container_init:
    package_pip: dumb-init>=1.2.5
    entrypoint: '["dumb-init"]'
    cmd: '["csh"]'

additional_build_files:
    - src: files/ansible.cfg
      dest: configs

additional_build_steps:
  prepend_base:
    - RUN echo This is a prepend base command!
    # Enable Non-default stream before packages provided by it can be installed. (optional)
    # - RUN $PKGMGR module enable postgresql:15 -y
    # - RUN $PKGMGR install -y postgresql
  prepend_galaxy:
    - COPY _build/configs/ansible.cfg /etc/ansible/ansible.cfg

  prepend_final: |
    RUN whoami
    RUN cat /etc/os-release
  append_final:
    - RUN echo This is a post-install command!
    - RUN ls -la /etc
```