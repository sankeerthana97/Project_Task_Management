// Dashboard functionality
document.addEventListener('DOMContentLoaded', function() {
    // Sidebar toggle
    const sidebarCollapse = document.getElementById('sidebarCollapse');
    const sidebar = document.getElementById('sidebar');
    
    if (sidebarCollapse) {
        sidebarCollapse.addEventListener('click', function() {
            sidebar.classList.toggle('active');
        });
    }

    // Check authentication
    checkAuth();

    // Load initial dashboard content
    loadDashboardContent();

    // Event listeners for navigation
    setupNavigationListeners();

    // Logout functionality
    setupLogout();
});

// Check if user is authenticated
function checkAuth() {
    const token = localStorage.getItem('authToken');
    if (!token) {
        window.location.href = '/pages/login.html';
        return;
    }

    // Verify token with backend
    fetch('/api/auth/verify', {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Invalid token');
        }
        return response.json();
    })
    .then(data => {
        // Update user info in UI
        document.getElementById('userName').textContent = data.name;
        // Load role-specific content
        loadRoleSpecificContent(data.role);
    })
    .catch(error => {
        console.error('Auth error:', error);
        localStorage.removeItem('authToken');
        window.location.href = '/pages/login.html';
    });
}

// Load dashboard content based on role
function loadRoleSpecificContent(role) {
    const mainContent = document.getElementById('mainContent');
    
    // Hide/show menu items based on role
    const menuItems = {
        'Manager': ['projectSubmenu', 'taskSubmenu', 'team-link', 'reports-link'],
        'TeamLead': ['projectSubmenu', 'taskSubmenu', 'team-link'],
        'Employee': ['taskSubmenu']
    };

    // Hide all menu items first
    document.querySelectorAll('#sidebar ul li').forEach(item => {
        if (item.id !== 'dashboard-link') {
            item.style.display = 'none';
        }
    });

    // Show role-specific menu items
    menuItems[role].forEach(itemId => {
        const item = document.getElementById(itemId);
        if (item) {
            item.parentElement.style.display = 'block';
        }
    });

    // Load role-specific dashboard content
    fetch(`/api/dashboard/${role.toLowerCase()}`)
        .then(response => response.json())
        .then(data => {
            mainContent.innerHTML = generateDashboardHTML(role, data);
            initializeCharts(data);
        })
        .catch(error => {
            console.error('Error loading dashboard:', error);
            mainContent.innerHTML = '<div class="alert alert-danger">Error loading dashboard content</div>';
        });
}

// Generate dashboard HTML based on role and data
function generateDashboardHTML(role, data) {
    let html = `
        <div class="row mb-4">
            <div class="col-12">
                <h2>${role} Dashboard</h2>
            </div>
        </div>
        <div class="row">
            <div class="col-md-3">
                <div class="stat-card">
                    <div class="stat-title">Total Projects</div>
                    <div class="stat-value">${data.totalProjects || 0}</div>
                    <div class="stat-icon"><i class="bi bi-folder"></i></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="stat-card">
                    <div class="stat-title">Active Tasks</div>
                    <div class="stat-value">${data.activeTasks || 0}</div>
                    <div class="stat-icon"><i class="bi bi-list-task"></i></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="stat-card">
                    <div class="stat-title">Team Members</div>
                    <div class="stat-value">${data.teamMembers || 0}</div>
                    <div class="stat-icon"><i class="bi bi-people"></i></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="stat-card">
                    <div class="stat-title">Completed Tasks</div>
                    <div class="stat-value">${data.completedTasks || 0}</div>
                    <div class="stat-icon"><i class="bi bi-check-circle"></i></div>
                </div>
            </div>
        </div>`;

    // Add role-specific content
    if (role === 'Manager') {
        html += generateManagerDashboard(data);
    } else if (role === 'TeamLead') {
        html += generateTeamLeadDashboard(data);
    } else {
        html += generateEmployeeDashboard(data);
    }

    return html;
}

// Generate role-specific dashboard content
function generateManagerDashboard(data) {
    return `
        <div class="row mt-4">
            <div class="col-md-8">
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Project Overview</h5>
                    </div>
                    <div class="card-body">
                        <div class="chart-container">
                            <canvas id="projectChart"></canvas>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Team Performance</h5>
                    </div>
                    <div class="card-body">
                        <div class="chart-container">
                            <canvas id="teamPerformanceChart"></canvas>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
}

function generateTeamLeadDashboard(data) {
    return `
        <div class="row mt-4">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Team Tasks</h5>
                    </div>
                    <div class="card-body">
                        <div class="chart-container">
                            <canvas id="teamTasksChart"></canvas>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Task Status</h5>
                    </div>
                    <div class="card-body">
                        <div class="chart-container">
                            <canvas id="taskStatusChart"></canvas>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
}

function generateEmployeeDashboard(data) {
    return `
        <div class="row mt-4">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title mb-0">My Tasks</h5>
                    </div>
                    <div class="card-body">
                        <div class="task-list">
                            ${data.tasks ? data.tasks.map(task => `
                                <div class="task-item">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <h6 class="mb-0">${task.title}</h6>
                                        <span class="badge bg-${getStatusBadgeColor(task.status)}">${task.status}</span>
                                    </div>
                                    <p class="text-muted mb-0 mt-2">${task.description}</p>
                                    <div class="progress mt-2">
                                        <div class="progress-bar" role="progressbar" style="width: ${task.progress}%"></div>
                                    </div>
                                </div>
                            `).join('') : 'No tasks assigned'}
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
}

// Utility functions
function getStatusBadgeColor(status) {
    const colors = {
        'Not Started': 'secondary',
        'In Progress': 'primary',
        'Completed': 'success',
        'On Hold': 'warning',
        'Cancelled': 'danger'
    };
    return colors[status] || 'secondary';
}

function setupNavigationListeners() {
    // Add click handlers for navigation items
    document.querySelectorAll('#sidebar a').forEach(link => {
        link.addEventListener('click', function(e) {
            if (!this.classList.contains('dropdown-toggle')) {
                e.preventDefault();
                const href = this.getAttribute('href');
                if (href && href !== '#') {
                    loadContent(href);
                }
            }
        });
    });
}

function setupLogout() {
    const logoutLink = document.getElementById('logoutLink');
    if (logoutLink) {
        logoutLink.addEventListener('click', function(e) {
            e.preventDefault();
            localStorage.removeItem('authToken');
            window.location.href = '/pages/login.html';
        });
    }
}

// Initialize charts (placeholder - implement with actual chart library)
function initializeCharts(data) {
    // This would be implemented with a charting library like Chart.js
    console.log('Initializing charts with data:', data);
} 