# Executar todos os testes
pytest tests/ -v

# Executar com relatório detalhado
pytest tests/ -v --tb=short

# Executar testes específicos
pytest tests/test_auth.py -v
pytest tests/test_tarefas.py -v

# Executar com cobertura (instalar pytest-cov primeiro)
pip install pytest-cov
pytest tests/ --cov=. --cov-report=html