from flask import Flask, request, jsonify, render_template, redirect, url_for, session
from flask_cors import CORS
from flask_session import Session
import json
import os
from datetime import datetime, timedelta
from hashlib import sha256

app = Flask(__name__, template_folder='../templates', static_folder='../static')
app.config['SECRET_KEY'] = 'tu_clave_secreta_aqui'
app.config['SESSION_TYPE'] = 'filesystem'
app.config['SESSION_PERMANENT'] = True
app.config['PERMANENT_SESSION_LIFETIME'] = timedelta(days=7)
app.config['SESSION_COOKIE_SECURE'] = False
app.config['SESSION_COOKIE_HTTPONLY'] = True
app.config['SESSION_COOKIE_SAMESITE'] = 'Lax'
app.config['SESSION_COOKIE_NAME'] = 'erp_session'
Session(app)

CORS(app, supports_credentials=True)

# Configuración de archivo de usuarios
USERS_FILE = os.path.join(os.path.dirname(__file__), '../data/users.json')

def ensure_users_file():
    """Asegura que existe el archivo de usuarios"""
    os.makedirs(os.path.dirname(USERS_FILE), exist_ok=True)
    if not os.path.exists(USERS_FILE):
        with open(USERS_FILE, 'w') as f:
            json.dump([], f)

def read_users():
    """Lee usuarios del archivo JSON"""
    ensure_users_file()
    try:
        with open(USERS_FILE, 'r') as f:
            return json.load(f)
    except:
        return []

def write_users(users):
    """Escribe usuarios al archivo JSON"""
    ensure_users_file()
    with open(USERS_FILE, 'w') as f:
        json.dump(users, f, indent=2)

def hash_password(password, salt):
    """Cifra contraseña con salt"""
    return sha256((salt + password).encode()).hexdigest()

def create_salt():
    """Crea un salt único"""
    import uuid
    return str(uuid.uuid4())

# Rutas de autenticación
@app.route('/login', methods=['GET', 'POST'])
def login():
    error = None
    success = session.pop('mensaje_exito', None)
    
    if request.method == 'POST':
        email = request.form.get('email')
        password = request.form.get('password')
        
        if not email or not password:
            error = 'Email y contraseña son requeridos'
        else:
            users = read_users()
            user = next((u for u in users if u['email'].lower() == email.lower()), None)
            
            if not user:
                error = 'No existe cuenta con ese correo'
            elif hash_password(password, user['salt']) != user['password_hash']:
                error = 'Contraseña incorrecta'
            else:
                session['user_email'] = user['email']
                session['user_nombre'] = user.get('nombre', user['email'])
                session.permanent = True
                return redirect('/dashboard')
    
    return render_template('login.html', error=error, success=success)

@app.route('/register', methods=['GET', 'POST'])
def register():
    if request.method == 'POST':
        email = request.form.get('email')
        password = request.form.get('password')
        confirm_password = request.form.get('confirm_password')
        nombre = request.form.get('nombre', '')
        
        if not email or not password or not confirm_password:
            return render_template('register.html', error='Todos los campos son requeridos')
        
        if password != confirm_password:
            return render_template('register.html', error='Las contraseñas no coinciden')
        
        if len(password) < 6:
            return render_template('register.html', error='La contraseña debe tener al menos 6 caracteres')
        
        users = read_users()
        if any(u['email'].lower() == email.lower() for u in users):
            return render_template('register.html', error='Ya existe una cuenta con ese correo')
        
        salt = create_salt()
        new_user = {
            'email': email,
            'nombre': nombre,
            'salt': salt,
            'password_hash': hash_password(password, salt),
            'fecha_creacion': datetime.now().isoformat()
        }
        
        users.append(new_user)
        write_users(users)
        
        session['user_email'] = email
        session['user_nombre'] = nombre or email
        session.permanent = True
        return redirect('/dashboard')
    
    return render_template('register.html')

@app.route('/recover-password', methods=['GET', 'POST'])
def recover_password():
    if request.method == 'POST':
        email = request.form.get('email')
        nueva_password = request.form.get('nueva_password')
        confirmar_password = request.form.get('confirmar_password')
        
        if not email or not nueva_password or not confirmar_password:
            return render_template('recover_password.html', error='Todos los campos son requeridos')
        
        if nueva_password != confirmar_password:
            return render_template('recover_password.html', error='Las contraseñas no coinciden')
        
        if len(nueva_password) < 6:
            return render_template('recover_password.html', error='La contraseña debe tener al menos 6 caracteres')
        
        users = read_users()
        user = next((u for u in users if u['email'].lower() == email.lower()), None)
        
        if not user:
            return render_template('recover_password.html', error='No existe una cuenta con ese correo')
        
        user['salt'] = create_salt()
        user['password_hash'] = hash_password(nueva_password, user['salt'])
        
        write_users(users)
        session['mensaje_exito'] = 'Contraseña actualizada exitosamente. Por favor, inicia sesión.'
        return redirect('/login')
    
    return render_template('recover_password.html')

@app.route('/logout', methods=['GET', 'POST'])
def logout():
    # Limpiar datos de sesión
    session.pop('user_email', None)
    session.pop('user_nombre', None)
    session.pop('profile_message', None)
    session.clear()
    # Redirigir a login
    response = redirect('/login')
    response.delete_cookie('erp_session')
    return response

@app.route('/profile-success')
def profile_success():
    if 'user_email' not in session:
        return redirect('/login')
    message = session.pop('profile_message', 'Datos actualizados exitosamente')
    return render_template('profile_success.html', message=message)

@app.route('/profile', methods=['GET', 'POST'])
def profile():
    if 'user_email' not in session:
        return redirect('/login')
    
    users = read_users()
    user = next((u for u in users if u['email'] == session['user_email']), None)
    
    if not user:
        return redirect('/login')
    
    if request.method == 'POST':
        nombre = request.form.get('nombre', '')
        new_password = request.form.get('new_password', '')
        confirm_password = request.form.get('confirm_password', '')
        
        # Validar nombre
        if not nombre:
            return render_template('profile.html', user=user, error='El nombre es requerido')
        
        # Validar contraseña si se intenta cambiar
        if new_password or confirm_password:
            if new_password != confirm_password:
                return render_template('profile.html', user=user, error='Las contraseñas no coinciden')
            if len(new_password) < 6:
                return render_template('profile.html', user=user, error='La contraseña debe tener al menos 6 caracteres')
            
            # Cambiar contraseña
            user['salt'] = create_salt()
            user['password_hash'] = hash_password(new_password, user['salt'])
        
        # Actualizar datos
        user['nombre'] = nombre
        
        write_users(users)
        
        # Actualizar sesión
        session['user_nombre'] = nombre or user['email']
        session['profile_message'] = 'Tu perfil ha sido actualizado exitosamente'
        
        return redirect('/profile-success')
    
    return render_template('profile.html', user=user)

@app.route('/dashboard')
def dashboard():
    if 'user_email' not in session:
        return redirect('/login')
    return render_template('dashboard.html', user_nombre=session.get('user_nombre'))

# Rutas API existentes
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