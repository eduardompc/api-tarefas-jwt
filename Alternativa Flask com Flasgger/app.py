from flask import Flask, request, jsonify
from flasgger import Swagger, swag_from

app = Flask(__name__)
swagger = Swagger(app)

tarefas = []
id_counter = 1

@app.route('/tarefas', methods=['POST'])
@swag_from({
    'parameters': [{
        'name': 'body',
        'in': 'body',
        'required': True,
        'schema': {
            'type': 'object',
            'properties': {
                'titulo': {'type': 'string'},
                'descricao': {'type': 'string'}
            }
        }
    }],
    'responses': {201: {'description': 'Tarefa criada'}}
})
def criar_tarefa():
    global id_counter
    data = request.json
    tarefa = {
        'id': id_counter,
        'titulo': data['titulo'],
        'descricao': data['descricao']
    }
    tarefas.append(tarefa)
    id_counter += 1
    return jsonify(tarefa), 201

@app.route('/tarefas', methods=['GET'])
def listar_tarefas():
    return jsonify(tarefas)

if __name__ == '__main__':
    app.run(debug=True)