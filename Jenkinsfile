@Library('standard-library') _ 
pipeline {
    agent {
        label 'imagechecker'
    }
    environment {
        REGISTRY = 'registry.guildswarm.org'
        TOOL_LABEL = "apigateway"
        ENVIRONMENT = 'testportal'
        REPO = "${env.BRANCH_NAME}"
        IMAGE = 'api_gateway'
        VAULT_CA_ROUTE = credentials('vault-cert-route')
        NAME_CERT = "vault-ca.crt"
    }
    stages {
        stage('Extract CA Certificate') {
            agent {
                label 'alpine_kubectl'
            }
            steps {
                script {
                    withCredentials([file(credentialsId: "kubernetes-${REPO}", variable: 'KUBECONFIG_FILE')]) {
                        sh "chmod u+w ${KUBECONFIG_FILE}"
                        sh "mv ${KUBECONFIG_FILE} ~/.kube/config"
                    }
                    sh "kubectl exec -n vault vault-0 -- vault read -field=certificate ${VAULT_CA_ROUTE} > ${NAME_CERT}"
                    sh "rm -f ~/.kube/config"
                    stash includes: 'super-ca.crt', name: 'ca-cert'
                }
            }
        }
        stage('Build Docker Images') {
            steps {
                script {
                    unstash 'ca-cert'
                    def version = readFile('version').trim()
                    env.VERSION = version
                    sh '''find . \\( -name "*.csproj" -o -name "*.sln" -o -name "NuGet.docker.config" \\) -print0 | tar -cvf projectfiles.tar -T -'''
                    try {
                        withCredentials([usernamePassword(credentialsId: "backend${REPO}", usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                            sh "docker login -u '${DOCKER_USERNAME}' -p '${DOCKER_PASSWORD}' ${REGISTRY}"
                            sh "docker build . \
                                --build-arg NAME_CERT=${NAME_CERT} \
                                --build-arg ENVIRONMENT='${ENVIRONMENT}' \
                                -t ${REGISTRY}/${REPO}/${IMAGE}:${version} \
                                -t ${REGISTRY}/${REPO}/${IMAGE}:latest"
                            sh 'docker logout'
                        }
                    } finally {
                        sh "rm -f projectfiles.tar"
                    }
                }
            }
        }
        stage('Push Docker Images') {
            steps {
                script {
                    if (env.CHANGE_ID == null) {
                        withCredentials([usernamePassword(credentialsId: "harbor-${REPO}", usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                            sh "docker login -u '${DOCKER_USERNAME}' -p '${DOCKER_PASSWORD}' ${REGISTRY}"
                            sh "docker push ${REGISTRY}/${REPO}/${IMAGE}:${version}"
                            sh "docker push ${REGISTRY}/${REPO}/${IMAGE}:latest"
                            sh 'docker logout'
                        }
                    } else {
                        echo "Avoiding push for PR"
                    }
                }
            }
        }
        stage('Remove Docker Images') {
            steps {
                script {
                    sh "docker rmi ${REGISTRY}/${REPO}/${IMAGE}:${version}"
                    sh "docker rmi ${REGISTRY}/${REPO}/${IMAGE}:latest"
                }
            }
        }
        stage('Delete Pods') {
            steps {
                script {
                    node('alpine_kubectl') {
                        sh 'mkdir -p ~/.kube/'
                        withCredentials([file(credentialsId: "kubernetes-${REPO}", variable: 'KUBECONFIG_FILE')]) {
                            // Move the credentials to a temporary location
                            sh "mv ${KUBECONFIG_FILE} ~/.kube/config"
                        }
                        sh "kubectl -n backend delete pods -l app=${TOOL_LABEL}"
                        // Clean up the kube config
                        sh "rm -f ~/.kube/config"
                    }
                }
            }
        }
    }
	post {
		always {
			sh 'rm -rf *'
		}
		failure {
			script{
				pga.slack_webhook("backend")
			}
		}
	
    }
}
