// Script para manejar mensajes de error y éxito en la página de perfil

document.addEventListener('DOMContentLoaded', function() {
  const mensajesDiv = document.getElementById('mensajes-pagina');
  
  if (!mensajesDiv) return;
  
  const error = mensajesDiv.getAttribute('data-error');
  const exito = mensajesDiv.getAttribute('data-exito');
  
  if (error) {
    mostrarMensaje(error, 'error');
  }
  
  if (exito) {
    mostrarMensaje(exito, 'exito');
  }
});

function mostrarMensaje(mensaje, tipo) {
  if (!mensaje) return;
  
  const div = document.createElement('div');
  div.className = `mensaje-${tipo}`;
  div.textContent = mensaje;
  
  const estilo = document.createElement('style');
  if (!document.querySelector('style[data-mensajes]')) {
    estilo.setAttribute('data-mensajes', 'true');
    estilo.textContent = `
      .mensaje-error {
        background-color: #ffebee;
        color: #a00;
        padding: 15px;
        border-radius: 6px;
        margin-bottom: 20px;
        font-size: 16px;
        border-left: 4px solid #a00;
        text-align: center;
      }
      
      .mensaje-exito {
        background-color: #e8f5e9;
        color: #2e7d32;
        padding: 15px;
        border-radius: 6px;
        margin-bottom: 20px;
        font-size: 16px;
        border-left: 4px solid #2e7d32;
        text-align: center;
      }
    `;
    document.head.appendChild(estilo);
  }
  
  const container = document.querySelector('.container') || document.querySelector('main');
  if (container) {
    container.insertBefore(div, container.firstChild);
  }
}
