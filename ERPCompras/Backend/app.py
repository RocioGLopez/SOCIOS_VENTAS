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

    if inicio:
        compras = [c for c in compras if c['fecha'] >= inicio]
    if fin:
        compras = [c for c in compras if c['fecha'] <= fin]

    return jsonify(compras)

@app.route('/contratos', methods=['GET', 'POST'])
def contratos():
    contratos_data = [
        {'id': 1, 'numero': 'CON-001', 'proveedor': 'Proveedor A', 'inicio': '2026-01-01', 'vencimiento': '2026-04-01', 'estado': 'Activo', 'documento': 'contrato1.pdf'},
        {'id': 2, 'numero': 'CON-002', 'proveedor': 'Proveedor B', 'inicio': '2026-02-01', 'vencimiento': '2026-03-28', 'estado': 'Por vencer', 'documento': 'contrato2.pdf'},
        {'id': 3, 'numero': 'CON-003', 'proveedor': 'Proveedor C', 'inicio': '2025-06-01', 'vencimiento': '2026-06-01', 'estado': 'Activo', 'documento': 'contrato3.pdf'},
        {'id': 4, 'numero': 'CON-004', 'proveedor': 'Proveedor D', 'inicio': '2025-01-01', 'vencimiento': '2026-02-01', 'estado': 'Vencido', 'documento': 'contrato4.pdf'},
    ]

    proveedor = request.args.get('proveedor', '')
    if proveedor:
        contratos_data = [c for c in contratos_data if proveedor.lower() in c['proveedor'].lower()]

    return jsonify(contratos_data)
@app.route('/ordenes', methods=['GET', 'POST'])
def ordenes():
    # Solicitudes aprobadas disponibles (Criterio 1)
    solicitudes_aprobadas = [
        {'id': 1, 'producto': 'Laptop Dell', 'cantidad': 5, 'solicitante': 'Pablo'},
        {'id': 2, 'producto': 'Sillas de oficina', 'cantidad': 10, 'solicitante': 'Maria'},
        {'id': 3, 'producto': 'Monitor LG', 'cantidad': 3, 'solicitante': 'Carlos'},
    ]

    ordenes_data = [
        {'id': 1, 'numero': 'ORD-0001', 'proveedor': 'Proveedor A', 'producto': 'Laptop Dell', 'cantidad': 5, 'monto': 15000, 'estado': 'Emitida'},
        {'id': 2, 'numero': 'ORD-0002', 'proveedor': 'Proveedor B', 'producto': 'Sillas de oficina', 'cantidad': 10, 'monto': 3500, 'estado': 'Emitida'},
    ]

    return jsonify({
        'solicitudes_aprobadas': solicitudes_aprobadas,
        'ordenes': ordenes_data
    })

if __name__ == '__main__':
    app.run(debug=True, host='127.0.0.1', port=5000)
    #hola