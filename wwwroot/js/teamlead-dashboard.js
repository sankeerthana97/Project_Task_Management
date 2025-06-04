// Team Lead Dashboard JavaScript

// API endpoints
const API_URL = {
    PROJECT: '/api/project',
    TASK: '/api/task',
    EMPLOYEE: '/api/employee'
};

// Load projects when page loads
document.addEventListener('DOMContentLoaded', function() {
    loadProjects();
    loadStatistics();
    loadEmployees();
});

// Load team lead's projects
async function loadProjects() {
    try {
        const response = await fetch(`${API_URL.PROJECT}?role=TeamLead`);
        const projects = await response.json();
        
        // Update project select dropdown
        const projectSelect = document.getElementById('projectSelect');
        projectSelect.innerHTML = '<option value="">Select a project...</option>';
        
        projects.forEach(project => {
            const option = document.createElement('option');
            option.value = project.id;
            option.textContent = project.title;
            projectSelect.appendChild(option);
        });

        // Update statistics
        updateStatistics(projects);
    } catch (error) {
        console.error('Error loading projects:', error);
    }
}

// Load team members
async function loadEmployees() {
    try {
        const response = await fetch(`${API_URL.EMPLOYEE}?role=Employee`);
        const employees = await response.json();
        
        // Update employee select dropdown
        const employeeSelect = document.getElementById('employeeSelect');
        employeeSelect.innerHTML = '<option value="">Select an employee...</option>';
        
        employees.forEach(employee => {
            const option = document.createElement('option');
            option.value = employee.id;
            option.textContent = `${employee.firstName} ${employee.lastName} (${employee.employeeProfile.skills})`;
            employeeSelect.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading employees:', error);
    }
}

// Update statistics cards
function updateStatistics(projects) {
    const myProjects = projects.length;
    const activeTasks = projects.reduce((total, project) => {
        return total + (project.tasks?.filter(t => t.status === 'InProgress').length || 0);
    }, 0);
    const teamMembers = document.getElementById('employeeSelect').options.length - 1;
    const upcomingDeadlines = projects.reduce((total, project) => {
        const today = new Date();
        return total + (project.tasks?.filter(t => 
            t.status !== 'Completed' && 
            new Date(t.dueDate) > today && 
            new Date(t.dueDate) <= new Date(today.setDate(today.getDate() + 7))
        ).length || 0);
    }, 0);

    document.getElementById('myProjects').textContent = myProjects;
    document.getElementById('activeTasks').textContent = activeTasks;
    document.getElementById('teamMembers').textContent = teamMembers;
    document.getElementById('upcomingDeadlines').textContent = upcomingDeadlines;
}

// Handle project selection
const projectSelect = document.getElementById('projectSelect');
projectSelect.addEventListener('change', async function() {
    const projectId = this.value;
    if (projectId) {
        await loadTasks(projectId);
    } else {
        document.getElementById('tasksSection').style.display = 'none';
    }
});

// Load tasks for selected project
async function loadTasks(projectId) {
    try {
        const response = await fetch(`${API_URL.TASK}?projectId=${projectId}`);
        const tasks = await response.json();
        
        // Update tasks list
        const tasksList = document.getElementById('tasksList');
        tasksList.innerHTML = `
            <div class="table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Title</th>
                            <th>Assigned To</th>
                            <th>Status</th>
                            <th>Due Date</th>
                            <th>Priority</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${tasks.map(task => `
                            <tr>
                                <td>${task.title}</td>
                                <td>${task.assignedTo.firstName} ${task.assignedTo.lastName}</td>
                                <td>
                                    <span class="badge bg-${getStatusBadgeClass(task.status)}">
                                        ${task.status}
                                    </span>
                                </td>
                                <td>${new Date(task.dueDate).toLocaleDateString()}</td>
                                <td>
                                    <span class="badge bg-${getPriorityBadgeClass(task.priority)}">
                                        ${task.priority}
                                    </span>
                                </td>
                                <td>
                                    <button class="btn btn-sm btn-primary" onclick="editTask(${task.id})">
                                        <i class="fas fa-edit"></i>
                                    </button>
                                    <button class="btn btn-sm btn-danger" onclick="deleteTask(${task.id})">
                                        <i class="fas fa-trash"></i>
                                    </button>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        `;

        document.getElementById('tasksSection').style.display = 'block';
    } catch (error) {
        console.error('Error loading tasks:', error);
    }
}

// Create task modal handlers
const createTaskModal = new bootstrap.Modal(document.getElementById('createTaskModal'));
document.getElementById('createTaskBtn').addEventListener('click', function() {
    createTaskModal.show();
});

// Handle task creation
const createTaskForm = document.getElementById('createTaskForm');
createTaskForm.addEventListener('submit', async function(e) {
    e.preventDefault();

    const projectId = document.getElementById('projectSelect').value;
    if (!projectId) {
        showError('Please select a project first');
        return;
    }

    const taskData = {
        projectId: parseInt(projectId),
        title: document.getElementById('taskTitle').value,
        description: document.getElementById('taskDescription').value,
        dueDate: document.getElementById('dueDate').value,
        priority: document.getElementById('priority').value,
        status: 'ToDo',
        employeeId: document.getElementById('employeeSelect').value
    };

    try {
        const response = await fetch(API_URL.TASK, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify(taskData)
        });

        if (response.ok) {
            createTaskModal.hide();
            loadTasks(projectId);
            showSuccess('Task created successfully!');
        } else {
            throw new Error('Failed to create task');
        }
    } catch (error) {
        showError('Error creating task: ' + error.message);
    }
});

// Helper functions
function getStatusBadgeClass(status) {
    switch (status) {
        case 'ToDo': return 'secondary';
        case 'InProgress': return 'primary';
        case 'Completed': return 'success';
        default: return 'warning';
    }
}

function getPriorityBadgeClass(priority) {
    switch (priority) {
        case 'High': return 'danger';
        case 'Medium': return 'warning';
        case 'Low': return 'info';
        default: return 'secondary';
    }
}

function showSuccess(message) {
    const alert = document.createElement('div');
    alert.className = 'alert alert-success alert-dismissible fade show';
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.querySelector('.container-fluid').insertBefore(alert, document.querySelector('.container-fluid').firstChild);
}

function showError(message) {
    const alert = document.createElement('div');
    alert.className = 'alert alert-danger alert-dismissible fade show';
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.querySelector('.container-fluid').insertBefore(alert, document.querySelector('.container-fluid').firstChild);
}

// Load statistics
async function loadStatistics() {
    try {
        const response = await fetch('/api/dashboard/teamlead');
        const stats = await response.json();
        
        document.getElementById('myProjects').textContent = stats.myProjects;
        document.getElementById('activeTasks').textContent = stats.activeTasks;
        document.getElementById('teamMembers').textContent = stats.teamMembers;
        document.getElementById('upcomingDeadlines').textContent = stats.upcomingDeadlines;
    } catch (error) {
        console.error('Error loading statistics:', error);
    }
}
