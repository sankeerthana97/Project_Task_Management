// Manager Dashboard JavaScript

// API endpoints
const API_URL = '/api/project';

// Load projects when page loads
document.addEventListener('DOMContentLoaded', function() {
    loadProjects();
    loadStatistics();
});

// Load all projects
async function loadProjects() {
    try {
        const response = await fetch(API_URL);
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

// Update statistics cards
function updateStatistics(projects) {
    const totalProjects = projects.length;
    const activeProjects = projects.filter(p => p.status === 'Active').length;
    
    document.getElementById('totalProjects').textContent = totalProjects;
    document.getElementById('activeProjects').textContent = activeProjects;
}

// Handle project selection
const projectSelect = document.getElementById('projectSelect');
projectSelect.addEventListener('change', async function() {
    const projectId = this.value;
    if (projectId) {
        await loadProjectDetails(projectId);
    } else {
        document.getElementById('projectDetails').style.display = 'none';
    }
});

// Load project details
async function loadProjectDetails(projectId) {
    try {
        const response = await fetch(`${API_URL}/${projectId}`);
        const project = await response.json();
        
        // Update project info
        const projectInfo = document.getElementById('projectInfo');
        projectInfo.innerHTML = `
            <h4>${project.title}</h4>
            <p><strong>Status:</strong> ${project.status}</p>
            <p><strong>Start Date:</strong> ${new Date(project.startDate).toLocaleDateString()}</p>
            <p><strong>End Date:</strong> ${new Date(project.endDate).toLocaleDateString()}</p>
            <p><strong>Criticality:</strong> ${project.criticality}</p>
        `;

        // Update project team
        const projectTeam = document.getElementById('projectTeam');
        projectTeam.innerHTML = `
            <h5>Project Team</h5>
            <div class="row">
                ${project.projectAssignments.map(assignment => `
                    <div class="col-md-4 mb-3">
                        <div class="card">
                            <div class="card-body">
                                <h6 class="card-title">${assignment.employee.firstName} ${assignment.employee.lastName}</h6>
                                <p class="card-text">Role: ${assignment.role}</p>
                                <p class="card-text">Skills: ${assignment.employee.employeeProfile.skills}</p>
                            </div>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;

        document.getElementById('projectDetails').style.display = 'block';
    } catch (error) {
        console.error('Error loading project details:', error);
    }
}

// Create project modal handlers
const createProjectModal = new bootstrap.Modal(document.getElementById('createProjectModal'));
document.getElementById('createProjectBtn').addEventListener('click', function() {
    createProjectModal.show();
});

// Handle project creation
const createProjectForm = document.getElementById('createProjectForm');
createProjectForm.addEventListener('submit', async function(e) {
    e.preventDefault();

    const projectData = {
        title: document.getElementById('projectTitle').value,
        description: document.getElementById('projectDescription').value,
        startDate: document.getElementById('startDate').value,
        endDate: document.getElementById('endDate').value,
        requirements: document.getElementById('requirements').value,
        skillsRequired: document.getElementById('skillsRequired').value,
        numberOfPeopleNeeded: parseInt(document.getElementById('peopleNeeded').value),
        criticality: document.getElementById('criticality').value,
        status: 'NotStarted'
    };

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify(projectData)
        });

        if (response.ok) {
            createProjectModal.hide();
            loadProjects();
            showSuccess('Project created successfully!');
        } else {
            throw new Error('Failed to create project');
        }
    } catch (error) {
        showError('Error creating project: ' + error.message);
    }
});

// Helper functions
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
        const response = await fetch('/api/dashboard/manager');
        const stats = await response.json();
        
        document.getElementById('totalProjects').textContent = stats.totalProjects;
        document.getElementById('activeProjects').textContent = stats.activeProjects;
        document.getElementById('pendingTasks').textContent = stats.pendingTasks;
        document.getElementById('completedTasks').textContent = stats.completedTasks;
    } catch (error) {
        console.error('Error loading statistics:', error);
    }
}
