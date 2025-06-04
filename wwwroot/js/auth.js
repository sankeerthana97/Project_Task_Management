// API endpoints
const API_URL = '/api/auth';
const LOGIN_ENDPOINT = `${API_URL}/login`;
const REGISTER_ENDPOINT = `${API_URL}/register`;

// Function to set up API request interceptor
function setupApiInterceptor(token) {
    // Add token to all API requests
    const originalFetch = window.fetch;
    window.fetch = async (...args) => {
        const [resource, config] = args;
        
        // Only add token to API requests
        if (resource.startsWith('/api/')) {
            const headers = config.headers || {};
            headers['Authorization'] = `Bearer ${token}`;
            config.headers = headers;
        }
        
        return originalFetch(...args);
    };
}

// Utility functions
function showError(element, message) {
    if (element.tagName === 'FORM') {
        let errorDiv = element.querySelector('.form-error-message');
        if (!errorDiv) {
            errorDiv = document.createElement('div');
            errorDiv.className = 'alert alert-danger form-error-message';
            element.insertBefore(errorDiv, element.firstChild);
        }
        errorDiv.textContent = message;
        return;
    }

    const existingError = element.parentNode.querySelector('.error-message');
    if (!existingError) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message text-danger small mt-1';
        errorDiv.textContent = message;
        element.classList.add('is-invalid');
        element.parentNode.appendChild(errorDiv);
    }
}

function clearErrors(form) {
    form.querySelectorAll('.error-message').forEach(error => error.remove());
    form.querySelectorAll('.is-invalid').forEach(input => input.classList.remove('is-invalid'));

    const formError = form.querySelector('.form-error-message');
    if (formError) formError.remove();
}

function showSuccess(message) {
    const successDiv = document.createElement('div');
    successDiv.className = 'alert alert-success';
    successDiv.textContent = message;
    const form = document.querySelector('form');
    if (form) {
        form.parentNode.insertBefore(successDiv, form);
        setTimeout(() => successDiv.remove(), 3000);
    }
}

// Safe JSON parse helper
async function safeParseJSON(response) {
    try {
        const text = await response.text();
        if (!text) return {};
        return JSON.parse(text);
    } catch {
        return {};
    }
}

// Login form handling
const loginForm = document.getElementById('loginForm');
if (loginForm) {
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        clearErrors(loginForm);

        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value;

        try {
            const response = await fetch(LOGIN_ENDPOINT, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
            });

            const data = await safeParseJSON(response);

            if (!response.ok) {
                throw new Error(data.message || 'Login failed');
            }

            if (data.token) {
                // Store token in localStorage
                localStorage.setItem('authToken', data.token);
                
                // Store token in cookie
                const cookieOptions = {
                    path: '/',
                    secure: false, // Set to true in production
                    sameSite: 'Lax'
                };
                document.cookie = `jwt=${encodeURIComponent(data.token)}; ${Object.entries(cookieOptions).map(([key, value]) => `${key}=${value}`).join('; ')}`;
                
                // Set up API request interceptor
                setupApiInterceptor(data.token);
            }

            const roles = data.user?.roles || [];
            const primaryRole = roles.length > 0 ? roles[0] : null;

            switch (primaryRole) {
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
            showError(loginForm, error.message);
        }
    });
}

// Registration form handling
const registerForm = document.getElementById('registerForm');
if (registerForm) {
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        clearErrors(registerForm);

        const firstName = document.getElementById('firstName').value.trim();
        const lastName = document.getElementById('lastName').value.trim();
        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        const role = document.getElementById('role').value;

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
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    firstName,
                    lastName,
                    email,
                    password,
                    role,
                }),
            });

            const data = await safeParseJSON(response);

            if (!response.ok) {
                throw new Error(data.message || 'Registration failed');
            }

            showSuccess('Registration successful! Redirecting to login...');
            setTimeout(() => {
                window.location.href = '/pages/login.html';
            }, 2000);
        } catch (error) {
            showError(registerForm, error.message);
        }
    });
}

// Password strength validation for registration form
const passwordInput = document.getElementById('password');
if (passwordInput) {
    passwordInput.addEventListener('input', function () {
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
