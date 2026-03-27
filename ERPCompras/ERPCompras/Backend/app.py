from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app, resources={r"/*": {"origins": "*"}})

@app.route('/compras/historial')
def historial():
    inicio = request.args.get('inicio')
    fin = request.args.get('fin')

    compras = [
        {'id': 1, 'fecha': '2026-03-01', 'proveedor': 'Proveedor A', 'monto': 1500, 'estado': 'Pagado'},
        {'id': 2, 'fecha': '2026-03-10', 'proveedor': 'Proveedor B', 'monto': 2500, 'estado': 'Pendiente'},
        {'id': 3, 'fecha': '2026-03-15', 'proveedor': 'Proveedor C', 'monto': 870,  'estado': 'Cancelado'},
        {'id': 4, 'fecha': '2026-03-20', 'proveedor': 'Proveedor A', 'monto': 4100, 'estado': 'Pagado'},
        {'id': 5, 'fecha': '2026-03-25', 'proveedor': 'Proveedor D', 'monto': 620,  'estado': 'Pendiente'},
    ]

    # Filtro por fechas si se envían parámetros
    if inicio:
        compras = [c for c in compras if c['fecha'] >= inicio]
    if fin:
        compras = [c for c in compras if c['fecha'] <= fin]

    return jsonify(compras)

if __name__ == '__main__':
    app.run(debug=True, host='127.0.0.1', port=5000)