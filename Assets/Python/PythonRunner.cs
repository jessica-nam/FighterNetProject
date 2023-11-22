// Class to call Python Scripts

using UnityEngine;

public class PythonRunner : MonoBehaviour
{
    void Start()
    {
        PythonCode();
    }

    void PythonCode()
    {
        string pyCode = @"print('FighterNet Python Output')
import sys
sys.path.insert(0, 'C:\Python38\Lib\site-packages')

import torch
import torch.nn as nn
import torch.nn.functional as F

meta_data = 5
# Actions + meta_data
num_actions = 14 + meta_data
# Health of each, distance (x,y), current state (including hitstun), opponent state, direction facing, meta_data
input_space = 2 + 2 + 14 + 14 + 1 + meta_data

print('Parameters:', 'Metadata:', meta_data, ' Num Actions:', num_actions, ' Input Space:', input_space)

class FighterNet(nn.Module):

    def __init__(self, input_space, action_space, meta_data_size):
        super(FighterNet, self).__init__()
        self.fc1 = nn.Linear(input_space, 64)
        self.fc2 = nn.Linear(64, 64)
        self.fc3 = nn.Linear(64, action_space)
        self.meta_data_size = meta_data_size

    def forward(self, x):
        x = F.relu(self.fc1(x))
        x = F.relu(self.fc2(x))
        x = self.fc3(x)
        return torch.argmax(x[:-self.meta_data_size]), x[self.meta_data_size:]

def combine(m1, m2):
  new_model = FighterNet(input_space, num_actions, meta_data)
  probability = .5

  m1_parameters = [param.data for param in m1.parameters()]
  m1_mean = torch.mean(torch.cat([param.view(-1) for param in m1_parameters]))
  m1_variance = torch.var(torch.cat([param.view(-1) for param in m1_parameters]))

  # Calculate the mean and variance of parameters for m2
  m2_parameters = [param.data for param in m2.parameters()]
  m2_mean = torch.mean(torch.cat([param.view(-1) for param in m2_parameters]))
  m2_variance = torch.var(torch.cat([param.view(-1) for param in m2_parameters]))


  for new_param, m1_param, m2_param in zip(new_model.parameters(), m1.parameters(), m2.parameters()):
    if torch.rand(1).item() < probability:
        new_param.data.copy_(m1_param.data) * torch.normal(m1_mean, torch.sqrt(m1_variance) / 2)
    else:
        new_param.data.copy_(m2_param.data) * torch.normal(m1_mean, torch.sqrt(m2_variance) / 2)
  return new_model

# Test scenarios
model1 = FighterNet(input_space, num_actions, meta_data)
model2 = FighterNet(input_space, num_actions, meta_data)

# Print model structures for verification
print('Model 1 structure:', model1)
print('Model 2 structure:', model2)

# Combine the models
combined_model = combine(model1, model2)

# Print the structure of the combined model
print('Combined Model structure:', combined_model)
";

        string output = PythonInterop.RunPythonCode(pyCode);
        if (!string.IsNullOrEmpty(output))
        {
            Debug.Log(output);
        }
    }
}
