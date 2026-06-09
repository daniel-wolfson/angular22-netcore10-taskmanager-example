import { Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { CreateTaskComponent } from './components/create-task/create-task.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskItem } from './models/task-item.model';
import { TaskService } from './services/task.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CreateTaskComponent, TaskListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  private readonly taskService = inject(TaskService);

  private readonly tasksResource = rxResource<TaskItem[], void>({
    stream: () => this.taskService.getTasks(),
  });

  readonly tasks = computed(() => this.tasksResource.value() ?? []);
  readonly loading = this.tasksResource.isLoading;

  handleCreated(task: TaskItem): void {
    this.tasksResource.update((tasks) => [task, ...(tasks ?? [])]);
  }

  handleMarkDone(id: string): void {
    this.taskService.markAsDone(id).subscribe((updatedTask) => {
      this.tasksResource.update((tasks) =>
        (tasks ?? []).map((task) => (task.id === updatedTask.id ? updatedTask : task))
      );
    });
  }
}
