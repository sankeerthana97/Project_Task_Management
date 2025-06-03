// API endpoints
const API_URL = '/api/auth';
const LOGIN_ENDPOINT = `${API_URL}/login`;
const REGISTER_ENDPOINT = `${API_URL}/register`;

// Utility functions
function showError(element, message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.textContent = message;
    element.classList.add('is-invalid');
    element.parentNode.appendChild(errorDiv);
}

function clearErrors(form) {
    form.querySelectorAll('.error-message').forEach(error => error.remove());
    form.querySelectorAll('.is-invalid').forEach(input => input.classList.remove('is-invalid'));
}

function showSuccess(message) {
    const successDiv = document.createElement('div');
    successDiv.className = 'alert alert-success';
    successDiv.textContent = message;
    document.querySelector('.card-body').insertBefore(successDiv, document.querySelector('form'));
    setTimeout(() => successDiv.remove(), 3000);
}

// Login form handling
const loginForm = document.getElementById('loginForm');
if (loginForm) {
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        clearErrors(loginForm);

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const rememberMe = document.getElementById('rememberMe').checked;

        try {
            const response = await fetch(LOGIN_ENDPOINT, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password, rememberMe })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Login failed');
            }

            // Store token if provided
            if (data.token) {
                localStorage.setItem('authToken', data.token);
            }

            // Redirect based on role
            switch (data.role) {
                case 'Manager':
                    window.location.href = '/pages/dashboard/manager-dashboard.html';
                    break;
                case 'TeamLead':
                    window.location.href = '/pages/dashboard/teamlead-dashboard.html';
                    break;
                case 'Employee':
                    window.location.href = '/pages/dashboard/employee-dashboard.html';
                    break;
                default:
                    window.location.href = '/pages/dashboard/employee-dashboard.html';
            }
        } catch (error) {
            showError(document.getElementById('email'), error.message);
        }
    });
}

// Registration form handling
const registerForm = document.getElementById('registerForm');
if (registerForm) {
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        clearErrors(registerForm);

        const firstName = document.getElementById('firstName').value;
        const lastName = document.getElementById('lastName').value;
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        const role = document.getElementById('role').value;

        // Password validation
        if (password !== confirmPassword) {
            showError(document.getElementById('confirmPassword'), 'Passwords do not match');
            return;
        }

        if (password.length < 8) {
            showError(document.getElementById('password'), 'Password must be at least 8 characters long');
            return;
        }

        try {
            const response = await fetch(REGISTER_ENDPOINT, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    firstName,
                    lastName,
                    email,
                    password,
                    role
                })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Registration failed');
            }

            showSuccess('Registration successful! Redirecting to login...');
            setTimeout(() => {
                window.location.href = '/pages/login.html';
            }, 2000);
        } catch (error) {
            showError(document.getElementById('email'), error.message);
        }
    });
}

// Password strength validation
const passwordInput = document.getElementById('password');
if (passwordInput) {
    passwordInput.addEventListener('input', function() {
        const password = this.value;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasLowerCase = /[a-z]/.test(password);
        const hasNumbers = /\d/.test(password);
        const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        if (password.length < 8) {
            this.setCustomValidity('Password must be at least 8 characters long');
        } else if (!hasUpperCase || !hasLowerCase || !hasNumbers || !hasSpecialChar) {
            this.setCustomValidity('Password must include uppercase, lowercase, number and special character');
        } else {
            this.setCustomValidity('');
        }
    });
} 