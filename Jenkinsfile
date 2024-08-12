pipeline {
    agent {
        label 'imagechecker'
    }
    environment {
        REGISTRY='registry.guildswarm.org'
        USER=credentials('user_auto')
        TOKEN=credentials('token_auto')
        TOOL_LABEL="apigateway"
        ENVIRONMENT = env.BRANCH_NAME ?: input message: 'Choose the environment to deploy', parameters: [choice(name: 'ENVIRONMENT', choices: ['testportal', 'staging', 'production'], description: 'Choose the environment to deploy')]
        REPO='testportal'
        IMAGE='api_gateway'
    }
    stages{
        stage('Build Docker Images') {
            steps {
                script {
                    container ('dockertainer'){
                          def version = readFile('version').trim()
                          env.VERSION = version
                          sh''' find . \\( -name "*.csproj" -o -name "*.sln" -o -name "NuGet.docker.config" \\) -print0 \
                           | tar -cvf projectfiles.tar -T -
                           '''
						  try {
						    withCredentials([[$class: 'UsernamePasswordMultiBinding', credentialsId: "harbor-dotnet", usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD']]) {
                                sh "docker login -u \'${DOCKER_USERNAME}' -p \'${DOCKER_PASSWORD}' ${REGISTRY}"
							    sh "docker build . --build-arg ENVIRONMENT='${ENVIRONMENT}' -t ${REGISTRY}/${REPO}/${IMAGE}:${version} -t ${REGISTRY}/${REPO}/${IMAGE}:latest"
							    sh 'docker logout'
						    }
						  } finally {
							    sh "rm -f projectfiles.tar"
							  }
                        }
                    }
                }
            }
        stage('Test Vulnerabilities'){
            steps{
                script{
                    container('dockertainer'){
                        if (env.CHANGE_ID == null) {
                            sh "trivy image --exit-code 1 --quiet ${REGISTRY}/${REPO}/${IMAGE}:latest"
                        } else {
                            echo "Avoiding Scan in PR"
                        }
                    }
                }
            }
        }
        stage('Push Docker Images') {
            steps {
                script {
                    container ('dockertainer'){
                        if (env.CHANGE_ID == null) {
                                withCredentials([[$class: 'UsernamePasswordMultiBinding', credentialsId: 'harbor-dotnet', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD']]) {
                                    sh "docker login -u \'${DOCKER_USERNAME}' -p \'${DOCKER_PASSWORD}' ${REGISTRY}"
                                    sh "docker push ${REGISTRY}/${REPO}/${IMAGE}:$version"
                                    sh "docker push ${REGISTRY}/${REPO}/${IMAGE}:latest"
                                    sh 'docker logout'
                                }
                            } else {
                                echo "Avoiding push for PR"
                            }
                        }
                    }
                }
            }
        stage('Remove Docker Images') {
            steps {
                script {
                    container ('dockertainer'){
                            sh "docker rmi ${REGISTRY}/${REPO}/${IMAGE}:$version"
                            sh "docker rmi ${REGISTRY}/${REPO}/${IMAGE}:latest"
                            }
                        }
                    }
                }
        stage('Delete Pods') {
            steps {
                script {
                    node ('kubectl-1.28'){
                        container ('kube'){
                            sh 'mkdir -p ~/.kube/'
                            withCredentials([file(credentialsId: 'backend-testportal', variable: 'KUBECONFIG_FILE')]) {
                            // Move the credentials to a temporary location
                            sh "mv ${KUBECONFIG_FILE} ~/.kube/config"
                            }
                            sh "cd /home/guildswarm/ && ./kubectl delete pods -l app=${TOOL_LABEL}"
                        }
                    }
                }
            }
        }
    }  
    post {
        always{
            sh 'rm -rf *'
        }
        //failure {
        //    script{
        //        pga.slack_webhook("backend")
        //    }
        //}
    }
}