// Employee Dashboard JavaScript

// API endpoints
const API_URL = {
    TASK: '/api/task',
    EMPLOYEE: '/api/employee',
    COMMENT: '/api/comment'
};

// Load data when page loads
document.addEventListener('DOMContentLoaded', function() {
    loadProfile();
    loadTasks();
    loadStatistics();
});

// Load employee profile
async function loadProfile() {
    try {
        const userId = localStorage.getItem('authToken');
        const response = await fetch(`${API_URL.EMPLOYEE}/${userId}`);
        const employee = await response.json();
        
        // Update profile info
        const profileInfo = document.getElementById('profileInfo');
        profileInfo.innerHTML = `
            <h5>${employee.firstName} ${employee.lastName}</h5>
            <p><strong>Role:</strong> ${employee.employeeProfile.currentRole}</p>
            <p><strong>Skills:</strong> ${employee.employeeProfile.skills}</p>
            <p><strong>Experience:</strong> ${employee.employeeProfile.totalYearsOfExperience} years</p>
            <p><strong>Responsibilities:</strong> ${employee.employeeProfile.responsibilities}</p>
        `;
    } catch (error) {
        console.error('Error loading profile:', error);
    }
}

// Load tasks
async function loadTasks() {
    try {
        const response = await fetch(`${API_URL.TASK}?role=Employee`);
        const tasks = await response.json();
        
        // Update tasks list
        const tasksList = document.getElementById('tasksList');
        tasksList.innerHTML = `
            <div class="table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Project</th>
                            <th>Title</th>
                            <th>Status</th>
                            <th>Due Date</th>
                            <th>Priority</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${tasks.map(task => `
                            <tr>
                                <td>${task.project.title}</td>
                                <td>${task.title}</td>
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
                                    <button class="btn btn-sm btn-info" onclick="viewTask(${task.id})">
                                        <i class="fas fa-eye"></i>
                                    </button>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        `;
    } catch (error) {
        console.error('Error loading tasks:', error);
    }
}

// View task details
function viewTask(taskId) {
    const modal = new bootstrap.Modal(document.getElementById('taskDetailsModal'));
    loadTaskDetails(taskId);
    modal.show();
}

// Load task details
async function loadTaskDetails(taskId) {
    try {
        const response = await fetch(`${API_URL.TASK}/${taskId}`);
        const task = await response.json();
        
        // Update task details
        const taskDetails = document.getElementById('taskDetails');
        taskDetails.innerHTML = `
            <h4>${task.title}</h4>
            <p><strong>Project:</strong> ${task.project.title}</p>
            <p><strong>Description:</strong> ${task.description}</p>
            <p><strong>Due Date:</strong> ${new Date(task.dueDate).toLocaleDateString()}</p>
            <p><strong>Priority:</strong> ${task.priority}</p>
            <p><strong>Status:</strong> ${task.status}</p>
        `;

        // Load comments
        await loadComments(taskId);
    } catch (error) {
        console.error('Error loading task details:', error);
    }
}

// Load comments
async function loadComments(taskId) {
    try {
        const response = await fetch(`${API_URL.COMMENT}?taskId=${taskId}`);
        const comments = await response.json();
        
        // Update comments section
        const commentsSection = document.getElementById('commentsSection');
        commentsSection.innerHTML = `
            <h5>Comments</h5>
            <div class="comments-list">
                ${comments.map(comment => `
                    <div class="comment-item">
                        <div class="comment-header">
                            <strong>${comment.createdBy.firstName} ${comment.createdBy.lastName}</strong>
                            <span class="comment-date">${new Date(comment.createdDate).toLocaleString()}</span>
                        </div>
                        <p>${comment.content}</p>
                    </div>
                `).join('')}
            </div>
        `;
    } catch (error) {
        console.error('Error loading comments:', error);
    }
}

// Handle comment submission
const commentForm = document.getElementById('commentForm');
commentForm.addEventListener('submit', async function(e) {
    e.preventDefault();

    const taskId = document.getElementById('taskDetailsModal').dataset.taskId;
    const comment = document.getElementById('comment').value;

    try {
        const response = await fetch(API_URL.COMMENT, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify({
                taskId: parseInt(taskId),
                content: comment
            })
        });

        if (response.ok) {
            loadComments(taskId);
            document.getElementById('comment').value = '';
            showSuccess('Comment added successfully!');
        } else {
            throw new Error('Failed to add comment');
        }
    } catch (error) {
        showError('Error adding comment: ' + error.message);
    }
});

// Update task status
const updateStatusBtn = document.getElementById('updateStatusBtn');
updateStatusBtn.addEventListener('click', async function() {
    const taskId = document.getElementById('taskDetailsModal').dataset.taskId;
    const status = prompt('Enter new status (ToDo/InProgress/Completed):');

    if (!status) return;

    try {
        const response = await fetch(`${API_URL.TASK}/${taskId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify({ status })
        });

        if (response.ok) {
            loadTasks();
            showSuccess('Task status updated successfully!');
        } else {
            throw new Error('Failed to update task status');
        }
    } catch (error) {
        showError('Error updating task status: ' + error.message);
    }
});

// Update statistics
async function loadStatistics() {
    try {
        const response = await fetch('/api/dashboard/employee');
        const stats = await response.json();
        
        document.getElementById('assignedProjects').textContent = stats.assignedProjects;
        document.getElementById('myTasks').textContent = stats.myTasks;
        document.getElementById('completedTasks').textContent = stats.completedTasks;
        document.getElementById('upcomingDeadlines').textContent = stats.upcomingDeadlines;
    } catch (error) {
        console.error('Error loading statistics:', error);
    }
}

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
